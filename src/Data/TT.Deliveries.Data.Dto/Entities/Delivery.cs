using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Enums;

namespace TT.Deliveries.Data.Dto.Entities;

public class Delivery
{
    public Delivery()
    {
        
    }

    public Delivery(CreateDeliveryDto createDeliveryDto)
    {
        State = createDeliveryDto.State;
        AccessWindow = createDeliveryDto.AccessWindow;
        Recipient = createDeliveryDto.Recipient;
        Order = createDeliveryDto.Order;
    }

    public Delivery(UpdateDeliveryDto updateDeliveryDto)
    {
        Id = updateDeliveryDto.Id;
        State = updateDeliveryDto.State;
    }
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public DeliveryState State { get; set; }
    public AccessWindow AccessWindow { get; set; }
    public Recipient Recipient { get; set; }
    public Order Order { get; set; }
}