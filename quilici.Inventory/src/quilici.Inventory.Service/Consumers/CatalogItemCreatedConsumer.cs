using MassTransit;
using quilici.Catalog.Contracts;
using quilici.Common;
using quilici.Inventory.Service.Entities;
using System.Threading.Tasks;

namespace quilici.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.itemId);

            if (item != null)
                return;

            item = new CatalogItem
            {
                Id = message.itemId,
                Name = message.name,
                Description = message.description,
            };

            await repository.CreateAsync(item);
        }
    }
}
