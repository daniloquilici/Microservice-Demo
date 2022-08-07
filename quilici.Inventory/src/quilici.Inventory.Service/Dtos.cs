using System;

namespace quilici.Inventory.Service.Dtos
{
    public record GrantItemsDto(Guid userId, Guid catalogItemId, int quantity);

    public record InventoryItemDto(Guid catalogItemId, string name, string description, int quantity, DateTimeOffset acquireDate);

    public record CatalogItemDto(Guid id, string name, string description);
}
