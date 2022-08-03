﻿using MongoDB.Driver;
using quilici.Catalog.Service.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace quilici.Catalog.Service.Repositories
{
    public class ItemsRepository
    {
        private const string collectionName = "items";

        private readonly IMongoCollection<Item> dbColletcion;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public ItemsRepository()
        {
            var mongoClient = new MongoClient("mongodb://192.168.115.128:27017");
            var database = mongoClient.GetDatabase("Catalog");
            dbColletcion = database.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync() 
        {
            return await dbColletcion.Find(filterBuilder.Empty).ToListAsync();   
        }

        public async Task<Item> GetAsync(Guid id) 
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbColletcion.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Item entity) 
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await dbColletcion.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Item entity) 
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            FilterDefinition<Item> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbColletcion.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id) 
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbColletcion.DeleteOneAsync(filter);
        }
    }
}
