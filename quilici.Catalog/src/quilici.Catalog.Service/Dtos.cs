using System;

namespace quilici.Catalog.Service
{
    public class Dtos
    {
        public record ItemDto(Guid id, string name, string description, decimal price, DateTimeOffset createDate);

        public record CreateItemDto(string name, string description, decimal price);

        public record UpdateItemDto(string name, string description, decimal price);
    }
}
