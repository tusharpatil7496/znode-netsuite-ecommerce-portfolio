using Znode.Core.DataContracts.Payment;

namespace Znode.Engine.Services.Payment
{
    public interface IPaymentGatewayService
    {
        Task<PaymentResponseModel> ProcessAndSavePaymentAsync(PaymentRequestModel request);
    }
}