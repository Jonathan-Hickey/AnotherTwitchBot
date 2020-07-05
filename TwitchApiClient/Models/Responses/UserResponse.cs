using System.Text.Json.Serialization;

namespace TwitchApiClient.Models.Responses
{
    public class UserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("broadcaster_type")]
        public string BroadcasterType { get; set; }
        
        [JsonPropertyName("description")] 
        public string Description { get; set; }

        [JsonPropertyName("Profile_image_url")]
        public string ProfileImageUrl { get; set; }
        
        [JsonPropertyName("offline_image_url")]
        public string OfflineImageUrl { get; set; }

        [JsonPropertyName("view_count")]
        public int ViewCount { get; set; }
    }
}
