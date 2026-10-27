using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Znode.Core.DataContracts.Inventory;

namespace Znode.Engine.Services.Inventory
{
    public class NetSuiteInventoryService : INetSuiteInventoryService
    {
        private readonly string _connectionString;

        public NetSuiteInventoryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ZnodeCustomDb")
                ?? throw new InvalidOperationException("Connection string 'ZnodeCustomDb' not found in configuration.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<NetSuiteInventoryModel?> GetBySkuAsync(string sku)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Sku", sku, DbType.String, ParameterDirection.Input);

            return await connection.QueryFirstOrDefaultAsync<NetSuiteInventoryModel>(
                "sp_GetInventoryBySku",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<NetSuiteInventoryModel>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber, DbType.Int32);
            parameters.Add("@PageSize", pageSize, DbType.Int32);

            return await connection.QueryAsync<NetSuiteInventoryModel>(
                "sp_GetAllInventory",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<NetSuiteInventoryModel> UpsertAsync(NetSuiteInventoryModel model)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Sku", model.Sku, DbType.String);
            parameters.Add("@NetSuiteItemId", model.NetSuiteItemId, DbType.String);
            parameters.Add("@QuantityAvailable", model.QuantityAvailable, DbType.Decimal);
            parameters.Add("@QuantityOnHand", model.QuantityOnHand, DbType.Decimal);
            parameters.Add("@WarehouseLocation", model.WarehouseLocation, DbType.String);

            return await connection.QuerySingleAsync<NetSuiteInventoryModel>(
                "sp_UpsertNetSuiteInventory",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeleteAsync(string sku)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Sku", sku, DbType.String);

            int deletedRows = await connection.ExecuteScalarAsync<int>(
                "sp_DeleteInventoryBySku",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return deletedRows > 0;
        }
    }
}