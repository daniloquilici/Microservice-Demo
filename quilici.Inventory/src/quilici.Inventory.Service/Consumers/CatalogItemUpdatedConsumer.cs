using MassTransit;
using quilici.Catalog.Contracts;
using quilici.Common;
using quilici.Inventory.Service.Entities;
using System.Threading.Tasks;

namespace quilici.Inventory.Service.Consumers
{
    public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdate>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdate> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.itemId);

            if (item == null)
            {
                item = new CatalogItem
                {
                    Id = message.itemId,
                    Name = message.name,
                    Description = message.description,
                };

                await repository.CreateAsync(item);
            }
            else
            {
                item.Name = message.name;
                item.Description = message.description;

                await repository.UpdateAsync(item);
            }
        }
    }
}
