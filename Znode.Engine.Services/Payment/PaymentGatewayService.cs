using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Znode.Core.DataContracts.Payment;

namespace Znode.Engine.Services.Payment
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly string _connectionString;

        public PaymentGatewayService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ZnodeCustomDb")
                ?? throw new InvalidOperationException("Connection string 'ZnodeCustomDb' not found in configuration.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<PaymentResponseModel> ProcessAndSavePaymentAsync(PaymentRequestModel request)
        {
            string prefix = request.GatewayType.Equals("Stripe", StringComparison.OrdinalIgnoreCase)
                ? "ch_stripe_"
                : "anet_txn_";

            var response = new PaymentResponseModel
            {
                TransactionId = $"{prefix}{Guid.NewGuid():N}".Substring(0, 22),
                Status = "Approved",
                AuthCode = "AUTH-" + Random.Shared.Next(100000, 999999),
                ProcessedAmount = request.Amount,
                ProcessedUtc = DateTime.UtcNow
            };

            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@OrderNumber", request.OrderNumber, DbType.String);
            parameters.Add("@GatewayType", request.GatewayType, DbType.String);
            parameters.Add("@TransactionId", response.TransactionId, DbType.String);
            parameters.Add("@AuthCode", response.AuthCode, DbType.String);
            parameters.Add("@ProcessedAmount", response.ProcessedAmount, DbType.Decimal);
            parameters.Add("@Currency", request.Currency, DbType.String);
            parameters.Add("@CustomerEmail", request.CustomerEmail, DbType.String);
            parameters.Add("@PaymentStatus", response.Status, DbType.String);

            await connection.ExecuteAsync(
                "sp_InsertPaymentTransaction",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return response;
        }
    }
}