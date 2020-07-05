using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TwitchApiClient.Options;

namespace TwitchApiClient
{
    internal interface IAuthenticationClient
    {
        Task<string> GetAccessTokenAsync();
    }

    internal class AuthenticationClient : IAuthenticationClient
    {
        private readonly IOptions<TwitchApiConfig> _twitchApiConfigOptions;
        
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public AuthenticationClient(HttpClient httpClient,
                                    IMemoryCache cache,
                                    IOptions<TwitchApiConfig> twitchApiConfigOptions)
        {
            _cache = cache;
            _httpClient = httpClient;
            _twitchApiConfigOptions = twitchApiConfigOptions;
        }
        
        private const string AccessTokenKey = "access_token";

        public async Task<string> GetAccessTokenAsync()
        {
            if (_cache.TryGetValue(AccessTokenKey, out string accessToken))
            {
                return accessToken;
            }

            var token = await GetTokenResponseAsync();

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(token.ExpiresIn));
                
            _cache.Set(AccessTokenKey, token.AccessToken, cacheEntryOptions);

            return token.AccessToken;
        }
        
        private async Task<TokenResponse> GetTokenResponseAsync()
        {
            // discover endpoints from metadata
            var disco = await _httpClient.GetDiscoveryDocumentAsync("https://id.twitch.tv/oauth2");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            // request token
            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = _twitchApiConfigOptions.Value.ClientId,
                ClientSecret = _twitchApiConfigOptions.Value.ClientSecret,
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return tokenResponse;
        }
    }
}
