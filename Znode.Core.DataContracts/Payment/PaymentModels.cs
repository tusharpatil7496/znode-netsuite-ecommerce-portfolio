namespace Znode.Core.DataContracts.Payment
{
    public class PaymentRequestModel
    {
        public string GatewayType { get; set; } = "Stripe";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string CardTokenOrNonce { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class PaymentResponseModel
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
        public decimal ProcessedAmount { get; set; }
        public DateTime ProcessedUtc { get; set; } = DateTime.UtcNow;
    }
}