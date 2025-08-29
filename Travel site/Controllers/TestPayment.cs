using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Travel_site.Controllers
{
    //    public class PaymobOrderTest
    //    {
    //        [JsonPropertyName("id")]
    //        public int Id { get; set; }

    //        [JsonPropertyName("merchant_order_id")]
    //        public string MerchantOrderId { get; set; }

    //        [JsonPropertyName("amount_cents")]
    //        public int AmountCents { get; set; }

    //        [JsonPropertyName("currency")]
    //        public string Currency { get; set; }

    //        [JsonPropertyName("created_at")]
    //        public DateTime CreatedAt { get; set; }

    //        // ... other order details can go here
    //    }
    //    public class PaymobCallbackWrapper
    //    {
    //        [JsonProperty("type")]
    //        public string Type { get; set; }

    //        [JsonProperty("obj")]
    //        public PaymobCallbacktest Obj { get; set; }
    //    }

    //    public class PaymobTransactionDataTest
    //    {
    //        [JsonPropertyName("message")]
    //        public string Message { get; set; }

    //        [JsonPropertyName("gateway_integration_pk")]
    //        public int GatewayIntegrationPk { get; set; }
    //        public int? TransactionId { get; set; }
    //        public string? Currency { get; set; }
    //        public string? IntegrationId { get; set; }
    //        public string? CardType { get; set; }
    //        public string? MaskedPan { get; set; }
    //        public string? Expiry { get; set; }
    //        public string? Bank { get; set; }
    //        public string? SubType { get; set; }
    //        public string? MerchantId { get; set; }

    //        // ... other transaction data like credit card details (masked)
    //    }

    //    public class PaymobSourceDataTest
    //    {
    //        [JsonPropertyName("type")]
    //        public string Type { get; set; } // e.g., "card"

    //        [JsonPropertyName("pan")]
    //        public string Pan { get; set; } // Masked card number

    //        [JsonPropertyName("sub_type")]
    //        public string SubType { get; set; } // e.g., "MasterCard"
    //    }

    //    //using System.Text.Json.Serialization;

    //    public class PaymobCallbacktest
    //    {

    //        public override string ToString()
    //        {
    //            return $@"
    //Id: {Id}
    //Pending: {Pending}
    //AmountCents: {AmountCents}
    //Success: {Success}
    //IsStandalonePayment: {IsStandalonePayment}
    //IsVoided: {IsVoided}
    //IsRefunded: {IsRefunded}
    //IsCaptured: {IsCaptured}
    //Is3dSecure: {Is3dSecure}
    //IntegrationId: {IntegrationId}
    //ProfileId: {ProfileId}
    //Order: {(Order == null ? "null" : $"Id={Order.Id}, MerchantOrderId={Order.MerchantOrderId}, AmountCents={Order.AmountCents}, Currency={Order.Currency}")}
    //CreatedAt: {CreatedAt}
    //Currency: {Currency ?? "null"}

    //ErrorOccured: {ErrorOccured}
    //HasParentTransaction: {HasParentTransaction}
    //Data: {(Data == null ? "null" : $"Message={Data.Message}, GatewayIntegrationPk={Data.GatewayIntegrationPk}")}
    //SourceData: {(SourceData == null ? "null" : $"Type={SourceData.Type}, Pan={SourceData.Pan}, SubType={SourceData.SubType}")}
    //ApiSource: {ApiSource ?? "null"}
    //Hmac: {Hmac ?? "null"}
    //IsCapture: {IsCapture}
    //";
    //        }

    //        [JsonPropertyName("id")]
    //        public int Id { get; set; }

    //        [JsonPropertyName("pending")]
    //        public bool Pending { get; set; }

    //        [JsonPropertyName("amount_cents")]
    //        public int AmountCents { get; set; }

    //        [JsonPropertyName("success")]
    //        public bool Success { get; set; }

    //        [JsonPropertyName("is_standalone_payment")]
    //        public bool IsStandalonePayment { get; set; }

    //        [JsonPropertyName("is_voided")]
    //        public bool IsVoided { get; set; }

    //        [JsonPropertyName("is_refunded")]
    //        public bool IsRefunded { get; set; }

    //        [JsonPropertyName("is_captured")]
    //        public bool IsCaptured { get; set; }

    //        [JsonPropertyName("is_3d_secure")]
    //        public bool Is3dSecure { get; set; }

    //        [JsonPropertyName("integration_id")]
    //        public int IntegrationId { get; set; }

    //        [JsonPropertyName("profile_id")]
    //        public int ProfileId { get; set; }

    //        [JsonPropertyName("order")]
    //        public PaymobOrderTest Order { get; set; }

    //        [JsonPropertyName("created_at")]
    //        public DateTime CreatedAt { get; set; }

    //        [JsonPropertyName("currency")]
    //        public string Currency { get; set; }

    //        [JsonPropertyName("error_occured")]
    //        public bool ErrorOccured { get; set; }

    //        [JsonPropertyName("has_parent_transaction")]
    //        public bool HasParentTransaction { get; set; }

    //        [JsonPropertyName("data")]
    //        public PaymobTransactionDataTest Data { get; set; }


    //        [JsonPropertyName("source_data")]
    //        public PaymobSourceDataTest SourceData { get; set; }

    //        [JsonPropertyName("api_source")]
    //        public string ApiSource { get; set; }

    //        // Not in JSON, injected manually
    //        public string Hmac { get; set; }

    //        [JsonPropertyName("is_capture")]
    //        public bool IsCapture { get; set; }
    //    }

    public class PaymobCallbackWrapper
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("obj")]
        public PaymobTransaction Obj { get; set; } = new();
    }
    public class PaymobTransaction
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("pending")]
        public bool Pending { get; set; }

        [JsonProperty("amount_cents")]
        public long AmountCents { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("is_auth")]
        public bool IsAuth { get; set; }

        [JsonProperty("is_capture")]
        public bool IsCapture { get; set; }

        [JsonProperty("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonProperty("is_voided")]
        public bool IsVoided { get; set; }

        [JsonProperty("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonProperty("is_3d_secure")]
        public bool Is3dSecure { get; set; }

        [JsonProperty("integration_id")]
        public int IntegrationId { get; set; }

        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonProperty("order")]
        public PaymobOrder Order { get; set; } = new();

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("data")]
        public PaymobData Data { get; set; } = new();

        [JsonProperty("source_data")]
        public SourceData? SourceData { get; set; }

        [JsonProperty("api_source")]
        public string? ApiSource { get; set; }

        [JsonProperty("error_occured")]
        public bool ErrorOccured { get; set; }

        [JsonProperty("is_captured")]
        public bool IsCaptured { get; set; }

        [Newtonsoft.Json.JsonIgnore] // we inject manually
        public string Hmac { get; set; } = string.Empty;
    }

    public class PaymobOrder
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("merchant_order_id")]
        public string MerchantOrderId { get; set; } = string.Empty;

        [JsonProperty("amount_cents")]
        public long AmountCents { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;
    }

    public class PaymobData
    {
        [JsonProperty("gateway_integration_pk")]
        public int GatewayIntegrationPk { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class SourceData
    {
        [JsonProperty("pan")]
        public string Pan { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("sub_type")]
        public string SubType { get; set; } = string.Empty;
    }


}
