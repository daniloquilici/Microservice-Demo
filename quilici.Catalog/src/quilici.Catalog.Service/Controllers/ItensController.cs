using Microsoft.AspNetCore.Mvc;
using quilici.Catalog.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static quilici.Catalog.Service.Dtos;

namespace quilici.Catalog.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensController : ControllerBase
    {
        private readonly ItemsRepository itemsRepository = new();

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
    }
}
