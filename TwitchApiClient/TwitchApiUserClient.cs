using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TwitchApiClient.Models.Responses;
using TwitchApiClient.Options;

namespace TwitchApiClient
{

    public interface ITwitchApiUserClient
    {
        Task<UserResponse> GetUser(string userName);
    }

    internal class TwitchApiUserClient : ITwitchApiUserClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationClient _authenticationClient;
        private readonly IOptions<TwitchApiConfig> _twitchApiConfigOptions;

        public TwitchApiUserClient(HttpClient httpClient, IAuthenticationClient authenticationClient, IOptions<TwitchApiConfig> twitchApiConfigOptions)
        {
            _twitchApiConfigOptions = twitchApiConfigOptions;
            _authenticationClient = authenticationClient;
            _httpClient = httpClient;
        }

        private const string GetUserUrl = "https://api.twitch.tv/helix/users?login={0}";
        private const string ClientId = "Client-ID";
        public async Task<UserResponse> GetUser(string userName)
        {
            var accessToken = await _authenticationClient.GetAccessTokenAsync();
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add(ClientId, _twitchApiConfigOptions.Value.ClientId);
            
            var httpResponse = await _httpClient.GetAsync(string.Format(GetUserUrl, userName));

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<TwitchResponse<UserResponse>>(response);

                return data.Data.SingleOrDefault();
            }
            
            throw new Exception($"Status Code : {httpResponse.StatusCode}  Reason Phrase : {httpResponse.ReasonPhrase}");
        }
    }
}
