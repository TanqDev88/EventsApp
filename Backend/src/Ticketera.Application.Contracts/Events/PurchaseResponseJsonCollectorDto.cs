using System.Text.Json.Serialization;

namespace Ticketera.Events
{
    public class PurchaseResponseJsonCollectorDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
    }
}
