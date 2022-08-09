using MassTransit;
using quilici.Catalog.Contracts;
using quilici.Common;
using quilici.Inventory.Service.Entities;
using System.Threading.Tasks;

namespace quilici.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDelete>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemDelete> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.itemId);

            if (item == null)
                return;

            await repository.RemoveAsync(message.itemId);
        }
    }
}
