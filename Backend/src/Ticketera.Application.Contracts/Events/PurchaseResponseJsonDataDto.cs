using System.Text.Json.Serialization;

namespace Ticketera.Events
{
    public class PurchaseResponseJsonDataDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
