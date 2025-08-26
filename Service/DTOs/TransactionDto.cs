using Domain.Entity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Travelsite.DTOs
{
    public class MemberShipInitiateTransactionDto
    {
        [Required]
        public int MembershipId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? CallbackUrl { get; set; }
        public string? ReturnUrl { get; set; }
    }
    public class TransactionDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? MembershipId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatus Status { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }

    public class InitiateTransactionDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? CallbackUrl { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class ConfirmTransactionDto
    {
        [Required]
        public string TransactionReference { get; set; } = string.Empty;

        [Required]
        public TransactionStatus Status { get; set; }

        public string? PaymobTransactionId { get; set; }
        public string? PaymobOrderId { get; set; }
        public decimal? ActualAmount { get; set; }
    }

    public class PaymobWebhookDto
    {
        public PaymobTransactionData? Obj { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class PaymobTransactionData
    {
        #region test

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("pending")]
        //public bool Pending { get; set; }

        //[JsonProperty("amount_cents")]
        //public long Amount_cents { get; set; }

        //[JsonProperty("success")]
        //public bool Success { get; set; }

        //[JsonProperty("is_auth")]
        //public bool Is_auth { get; set; }

        //[JsonProperty("is_capture")]
        //public bool Is_capture { get; set; }

        //[JsonProperty("is_standalone_payment")]
        //public bool Is_standalone_payment { get; set; }

        //[JsonProperty("is_voided")]
        //public bool Is_voided { get; set; }

        //[JsonProperty("is_refunded")]
        //public bool Is_refunded { get; set; }

        //[JsonProperty("is_3d_secure")]
        //public bool Is_3d_secure { get; set; }

        //[JsonProperty("integration_id")]
        //public long Integration_id { get; set; }

        //[JsonProperty("profile_id")]
        //public long Profile_id { get; set; }

        //[JsonProperty("has_parent_transaction")]
        //public bool Has_parent_transaction { get; set; }

        //[JsonProperty("order")]
        //public PaymobOrderDto Order { get; set; }

        //[JsonProperty("created_at")]
        //public DateTime Created_at { get; set; }

        //[JsonProperty("currency")]
        //public string Currency { get; set; }

        //[JsonProperty("merchant_commission")]
        //public int Merchant_commission { get; set; }

        //[JsonProperty("discount_details")]
        //public List<object> Discount_details { get; set; }

        //[JsonProperty("is_void")]
        //public bool Is_void { get; set; }

        //[JsonProperty("is_refund")]
        //public bool Is_refund { get; set; }

        //[JsonProperty("error_occured")]
        //public bool Error_occured { get; set; }

        //[JsonProperty("refunded_amount_cents")]
        //public int Refunded_amount_cents { get; set; }

        //[JsonProperty("captured_amount")]
        //public int Captured_amount { get; set; }

        //[JsonProperty("updated_at")]
        //public DateTime Updated_at { get; set; }

        //[JsonProperty("is_settled")]
        //public bool Is_settled { get; set; }

        //[JsonProperty("bill_balanced")]
        //public bool Bill_balanced { get; set; }

        //[JsonProperty("is_bill")]
        //public bool Is_bill { get; set; }

        //[JsonProperty("owner")]
        //public long Owner { get; set; }

        //[JsonProperty("merchant_order_id")]
        //public string Merchant_order_id { get; set; }

        //[JsonProperty("data.message")]
        //public string DataMessage { get; set; }

        //[JsonProperty("source_data")]
        //public PaymobSourceDataDto Source_data { get; set; }

        //[JsonProperty("acq_response_code")]
        //public string Acq_response_code { get; set; }

        //[JsonProperty("txn_response_code")]
        //public object Txn_response_code { get; set; }

        #endregion

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("amount_cents")]
        public decimal Amount_cents { get; set; }
        public PaymobOrderDto? Order { get; set; }

        //[JsonProperty("txn_response_code")]
        //public string? Txn_response_code { get; set; }

        public string? Gateway_response_code { get; set; }
        public string? Gateway_response_message { get; set; }
    }

    #region test

    public class PaymobSourceDataDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("pan")]
        public string Pan { get; set; }

        [JsonProperty("sub_type")]
        public string Sub_type { get; set; }
    } 
    #endregion


    public class PaymobOrderDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        public string? Merchant_order_id { get; set; }
        public decimal Amount_cents { get; set; }
    }

    public class PaymobAuthResponse
    {
        [JsonPropertyName("token")] // Use this if JSON key is "token"
        public string Token { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        // Add other properties matching the JSON

    }
    public class PaymobErrorResponse
    {
        public int Status { get; set; }
        public string Detail { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
    public class PaymobOrderResponse
    {
        public int Id { get; set; }
        public decimal Amount_cents { get; set; }
        public string Merchant_order_id { get; set; } = string.Empty;
    }

    public class PaymobPaymentKeyResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
