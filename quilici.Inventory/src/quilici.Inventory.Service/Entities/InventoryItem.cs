using quilici.Common;
using System;

namespace quilici.Inventory.Service.Entities
{
    public class InventoryItem : IEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CatalagItemId { get; set; }

        public int Quantity { get; set; }

        public DateTimeOffset AcquireDate { get; set; }
    }
}
