using System;

namespace quilici.Catalog.Service.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}