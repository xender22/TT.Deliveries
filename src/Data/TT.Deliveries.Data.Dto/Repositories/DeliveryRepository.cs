using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Entities;
using TT.Deliveries.Data.Dto.Enums;
using TT.Deliveries.Data.Dto.Settings;

namespace TT.Deliveries.Data.Dto.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly IMongoCollection<Delivery> _deliveryCollection;

    public DeliveryRepository(IConfiguration config)
    {
        var settings = config.GetSection("MongoDbSettings").Get<MongoSettings>();
        var clientSettings = MongoClientSettings.FromConnectionString(settings?.ConnectionString);
        
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(settings?.DatabaseName);
        
        _deliveryCollection = database.GetCollection<Delivery>("Deliveries");
    }

    public async Task<IEnumerable<Delivery>> GetAll()
    {
        var deliveries = await _deliveryCollection.Find(_ => true).ToListAsync();
        return deliveries;
    }
    
    public async Task<string> CreateDelivery(CreateDeliveryDto createDeliveryDto)
    {
        var delivery = new Delivery(createDeliveryDto);
        await _deliveryCollection.InsertOneAsync(delivery);
        return delivery.Id;
    }

    public async Task<Delivery> GetDelivery(string id)
    {
        var filter = Builders<Delivery>.Filter.Eq(d => d.Id, id);
        return await _deliveryCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateDelivery(UpdateDeliveryDto updateDeliveryDto)
    {
        var delivery = new Delivery(updateDeliveryDto);
        var filter = Builders<Delivery>.Filter.Eq(d => d.Id, delivery.Id);

        var update = Builders<Delivery>.Update
            .Set(d => d.State, delivery.State);
        // Add more Set calls for each field you want to update
        // .Set(d => d.AnotherField, delivery.AnotherField)

        await _deliveryCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteDelivery(string id)
    {
        var filter = Builders<Delivery>.Filter.Eq(d => d.Id, id);
        await _deliveryCollection.DeleteOneAsync(filter);
    }
    
    public async Task<IEnumerable<Delivery>> GetExpiredDeliveries(DateTime currentTime)
    {
        var filter = Builders<Delivery>.Filter.And(
            Builders<Delivery>.Filter.In(d => d.State, new[] { DeliveryState.Created, DeliveryState.Approved }),
            Builders<Delivery>.Filter.Lt(d => d.AccessWindow.EndTime, currentTime)
        );

        return await _deliveryCollection.Find(filter).ToListAsync();
    }
    
    public async Task UpdateDeliveries(List<UpdateDeliveryDto> updateDeliveryDtos)
    {
        var replacementTasks = updateDeliveryDtos.Select(async dto =>
        {
            var delivery = new Delivery(dto);

            var updateBuilder = Builders<Delivery>.Update;
            var update = updateBuilder
                .Set(d => d.State, delivery.State);
            // Add more Set calls for each field you want to update
            // .Set(d => d.AnotherField, delivery.AnotherField);

            var filter = Builders<Delivery>.Filter.Eq(d => d.Id, delivery.Id);

            var updateOptions = new UpdateOptions { IsUpsert = true };

            await _deliveryCollection.UpdateOneAsync(filter, update, updateOptions);
        });

        await Task.WhenAll(replacementTasks);
    }

}