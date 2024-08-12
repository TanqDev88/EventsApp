using System.Text.Json.Serialization;

namespace Ticketera.Events
{
    public class PurchaseCheckDto
    {
        [JsonPropertyName("collection_id")]
        public string CollectionId { get; set; } = string.Empty;

        [JsonPropertyName("collection_status")]
        public string CollectionStatus { get; set; } = string.Empty;

        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; } = string.Empty;

        [JsonPropertyName("payment_type")]
        public string PaymentType { get; set; } = string.Empty;

        [JsonPropertyName("merchant_order_id")]
        public string MerchantOrderId { get; set; } = string.Empty;

        [JsonPropertyName("preference_id")]
        public string PreferenceId { get; set; } = string.Empty;

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; } = string.Empty;

        [JsonPropertyName("processing_mode")]
        public string ProcessingMode { get; set; } = string.Empty;

        [JsonPropertyName("merchant_account_id")]
        public string MerchantAccountId { get; set; } = string.Empty;
    }

}
