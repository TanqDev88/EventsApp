using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ticketera.Events
{
    public class PurchaseResponseJsonDto
    {
        [JsonPropertyName("resource")]
        public string Resource { get; set; } = string.Empty;
        
        [JsonPropertyName("topic")]
        public string Topic { get; set; } = string.Empty;

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public PurchaseResponseJsonDataDto? Data { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("live_mode")]
        public bool LiveMode { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;
    }
}
