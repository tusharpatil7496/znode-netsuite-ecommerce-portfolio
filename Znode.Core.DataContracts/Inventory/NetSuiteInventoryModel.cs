namespace Znode.Core.DataContracts.Inventory
{
    public class NetSuiteInventoryModel
    {
        public int InventoryId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string NetSuiteItemId { get; set; } = string.Empty;
        public decimal QuantityAvailable { get; set; }
        public decimal QuantityOnHand { get; set; }
        public string WarehouseLocation { get; set; } = string.Empty;
        public DateTime LastSyncedAt { get; set; }
    }
}