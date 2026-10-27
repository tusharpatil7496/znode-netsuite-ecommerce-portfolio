using Microsoft.AspNetCore.Mvc;
using Znode.Core.DataContracts.Common;
using Znode.Core.DataContracts.Inventory;
using Znode.Engine.Services.Inventory;

namespace Znode.Custom.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly INetSuiteInventoryService _inventoryService;

        public InventoryController(INetSuiteInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<NetSuiteInventoryModel>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var data = await _inventoryService.GetAllAsync(page, pageSize);
            return Ok(ApiResponse<IEnumerable<NetSuiteInventoryModel>>.Ok(data, "Inventory retrieved from database."));
        }

        [HttpGet("{sku}")]
        public async Task<ActionResult<ApiResponse<NetSuiteInventoryModel>>> GetBySku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return BadRequest(ApiResponse<NetSuiteInventoryModel>.Fail("SKU must be provided."));

            var item = await _inventoryService.GetBySkuAsync(sku);
            if (item == null)
                return NotFound(ApiResponse<NetSuiteInventoryModel>.Fail($"Inventory with SKU '{sku}' not found in database."));

            return Ok(ApiResponse<NetSuiteInventoryModel>.Ok(item, "Found record successfully."));
        }

        [HttpPost("sync")]
        public async Task<ActionResult<ApiResponse<NetSuiteInventoryModel>>> Upsert([FromBody] NetSuiteInventoryModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku))
                return BadRequest(ApiResponse<NetSuiteInventoryModel>.Fail("SKU cannot be blank."));

            var saved = await _inventoryService.UpsertAsync(request);
            return Ok(ApiResponse<NetSuiteInventoryModel>.Ok(saved, "Inventory synchronized and stored in database via SP."));
        }

        [HttpDelete("{sku}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string sku)
        {
            bool deleted = await _inventoryService.DeleteAsync(sku);
            if (!deleted)
                return NotFound(ApiResponse<bool>.Fail($"SKU '{sku}' does not exist or was not deleted."));

            return Ok(ApiResponse<bool>.Ok(true, $"SKU '{sku}' was deleted from the database."));
        }
    }
}