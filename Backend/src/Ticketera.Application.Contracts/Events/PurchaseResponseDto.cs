using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ticketera.Events
{
    public class PurchaseResponseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; }

        [JsonPropertyName("preference_id")]
        public string PreferenceId { get; set; }

        [JsonPropertyName("payments")]
        public List<object> Payments { get; set; }

        [JsonPropertyName("shipments")]
        public List<object> Shipments { get; set; }

        [JsonPropertyName("payouts")]
        public List<object> Payouts { get; set; }

        [JsonPropertyName("collector")]
        public PurchaseResponseJsonCollectorDto Collector { get; set; }

        [JsonPropertyName("marketplace")]
        public string Marketplace { get; set; }

        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("sponsor_id")]
        public object SponsorId { get; set; }

        [JsonPropertyName("shipping_cost")]
        public int ShippingCost { get; set; }

        [JsonPropertyName("total_amount")]
        public int TotalAmount { get; set; }

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; }

        [JsonPropertyName("paid_amount")]
        public int PaidAmount { get; set; }

        [JsonPropertyName("refunded_amount")]
        public int RefundedAmount { get; set; }

        [JsonPropertyName("payer")]
        public object Payer { get; set; }

        [JsonPropertyName("items")]
        public List<PurchaseResponseJsonItemDto> Items { get; set; }

        [JsonPropertyName("cancelled")]
        public bool Cancelled { get; set; }

        [JsonPropertyName("additional_info")]
        public string AdditionalInfo { get; set; }

        [JsonPropertyName("application_id")]
        public object ApplicationId { get; set; }

        [JsonPropertyName("is_test")]
        public bool IsTest { get; set; }

        [JsonPropertyName("order_status")]
        public string OrderStatus { get; set; }
    }
}
