using Znode.Core.DataContracts.Inventory;

namespace Znode.Engine.Services.Inventory
{
    public interface INetSuiteInventoryService
    {
        Task<NetSuiteInventoryModel?> GetBySkuAsync(string sku);
        Task<IEnumerable<NetSuiteInventoryModel>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
        Task<NetSuiteInventoryModel> UpsertAsync(NetSuiteInventoryModel model);
        Task<bool> DeleteAsync(string sku);
    }
}