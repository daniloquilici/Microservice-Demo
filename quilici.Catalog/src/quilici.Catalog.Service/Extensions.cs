using quilici.Catalog.Service.Entities;
using static quilici.Catalog.Service.Dtos;

namespace quilici.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item) 
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
