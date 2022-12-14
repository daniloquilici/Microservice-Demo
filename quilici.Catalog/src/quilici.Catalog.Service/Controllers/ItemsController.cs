using MassTransit;
using Microsoft.AspNetCore.Mvc;
using quilici.Catalog.Service.Entities;
using quilici.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quilici.Catalog.Contracts;
using static quilici.Catalog.Service.Dtos;

namespace quilici.Catalog.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
                return NotFound();

            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.name,
                Description = createItemDto.description,
                Price = createItemDto.price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);
            if (existingItem == null)
                return NotFound();

            existingItem.Name = itemDto.name;
            existingItem.Description = itemDto.description;
            existingItem.Price = itemDto.price;

            await itemsRepository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
                return NotFound();

            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();

        }
    }
}
