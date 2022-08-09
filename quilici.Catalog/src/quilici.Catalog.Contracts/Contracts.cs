using System;

namespace quilici.Catalog.Contracts
{
    public class Contracts
    {
        public record CatalogItemCreated(Guid itemId, string name, string description);

        public record CatalogItemUpdate(Guid itemId, string name, string description);

        public record CatalogItemDelete(Guid itemId);
    }
}
