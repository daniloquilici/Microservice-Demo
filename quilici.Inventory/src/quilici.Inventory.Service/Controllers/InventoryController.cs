using Microsoft.AspNetCore.Mvc;
using quilici.Common;
using quilici.Inventory.Service.Clients;
using quilici.Inventory.Service.Dtos;
using quilici.Inventory.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quilici.Inventory.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryRepository;
        private readonly CatalogClient catalogClient;


        public InventoryController(IRepository<InventoryItem> inventoryRepository, CatalogClient catalogClient)
        {
            this._inventoryRepository = inventoryRepository;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var catalogItems = await catalogClient.GetCatalogItemAsync();
            var inventoryItemEntities = await _inventoryRepository.GetAllAsync(entity => entity.UserId == userId);

            var inventoryItemDto = inventoryItemEntities.Select(inventoryItem => 
            {
                var catalogItem = catalogItems.Single(x => x.id == inventoryItem.CatalagItemId);
                return inventoryItem.AsDto(catalogItem.name, catalogItem.description);
            });

            return Ok(inventoryItemDto);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto) 
        {
            var inventoryItem = await _inventoryRepository.GetAsync(item => item.UserId == grantItemsDto.userId && item.CatalagItemId == grantItemsDto.catalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalagItemId = grantItemsDto.catalogItemId,
                    Quantity = grantItemsDto.quantity,
                    UserId = grantItemsDto.userId,
                    AcquireDate = DateTimeOffset.UtcNow
                };

                await _inventoryRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.quantity;
                await _inventoryRepository.UpdateAsync(inventoryItem);
            }

            return Ok(inventoryItem);
        }
    }
}
