using quilici.Inventory.Service.Dtos;
using quilici.Inventory.Service.Entities;

namespace quilici.Inventory.Service
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDto(item.CatalagItemId, name, description, item.Quantity, item.AcquireDate);
        }
    }
}
