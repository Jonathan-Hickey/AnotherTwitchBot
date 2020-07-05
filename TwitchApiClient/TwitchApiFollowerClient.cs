using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TwitchApiClient.Models.Responses;
using TwitchApiClient.Options;

namespace TwitchApiClient
{
    public class FollowerResponse
    {
        [JsonPropertyName("from_id")]
        public string FromId { get; set; }

        [JsonPropertyName("from_name")]
        public string FromName { get; set; }

        [JsonPropertyName("to_id")]
        public string ToId { get; set; }

        [JsonPropertyName("to_name")]
        public string ToName { get; set; }

        [JsonPropertyName("followed_at")]
        public DateTime StartedFollowingAt { get; set; }
    }

    public interface ITwitchApiFollowerClient
    {
        Task<bool> IsUserAFollower(string userId);
    }

    internal class TwitchApiFollowerClient : ITwitchApiFollowerClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationClient _authenticationClient;
        private readonly IOptions<TwitchApiConfig> _twitchApiConfigOptions;

        public TwitchApiFollowerClient(HttpClient httpClient, IAuthenticationClient authenticationClient, IOptions<TwitchApiConfig> twitchApiConfigOptions)
        {
            _twitchApiConfigOptions = twitchApiConfigOptions;
            _authenticationClient = authenticationClient;
            _httpClient = httpClient;
        }


        private const string ClientId = "Client-ID";

        public async Task<bool> IsUserAFollower(string userId)
        {
            var accessToken = await _authenticationClient.GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add(ClientId, _twitchApiConfigOptions.Value.ClientId);

            var getFollowerUrl = $"https://api.twitch.tv/helix/users/follows?to_id=32115039&from_id={userId}&first=1";
            var httpResponse = await _httpClient.GetAsync(getFollowerUrl);

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync();

                var followers = JsonSerializer.Deserialize<TwitchResponse<FollowerResponse>>(response);

                return followers.Data.SingleOrDefault() != null;
            }

            throw new Exception($"Status Code : {httpResponse.StatusCode}  Reason Phrase : {httpResponse.ReasonPhrase}");

        }
    }
}
