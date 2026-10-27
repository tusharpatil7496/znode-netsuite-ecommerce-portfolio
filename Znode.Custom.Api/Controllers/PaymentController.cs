using Microsoft.AspNetCore.Mvc;
using Znode.Core.DataContracts.Common;
using Znode.Core.DataContracts.Payment;
using Znode.Engine.Services.Payment;

namespace Znode.Custom.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentService;

        public PaymentController(IPaymentGatewayService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<ActionResult<ApiResponse<PaymentResponseModel>>> ProcessPayment([FromBody] PaymentRequestModel request)
        {
            if (request.Amount <= 0)
                return BadRequest(ApiResponse<PaymentResponseModel>.Fail("Amount must be greater than zero."));

            var response = await _paymentService.ProcessAndSavePaymentAsync(request);
            return Ok(ApiResponse<PaymentResponseModel>.Ok(response, "Payment executed and transaction persisted in SQL Server."));
        }
    }
}