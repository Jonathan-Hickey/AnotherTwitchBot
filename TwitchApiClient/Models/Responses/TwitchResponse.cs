using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TwitchApiClient.Models.Responses
{
    internal class TwitchResponse<T>
    {
        [JsonPropertyName("data")]
        public IReadOnlyList<T> Data { get; set; }
    }
}
