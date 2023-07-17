using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Entities;
using TT.Deliveries.Data.Dto.Enums;
using TT.Deliveries.Data.Dto.Repositories;

namespace TT.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;

    public DeliveryService(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<string> CreateDelivery(CreateDeliveryDto createDeliveryDto)
    {
        var result = await _deliveryRepository.CreateDelivery(createDeliveryDto);
        return result;
    }

    public async Task UpdateDelivery(UpdateDeliveryDto updateDeliveryDto)
    {
        await _deliveryRepository.UpdateDelivery(updateDeliveryDto);
    }

    public async Task<Delivery> GetDelivery(string id)
    {
        var result = await _deliveryRepository.GetDelivery(id);
        return result;
    }

    public async Task DeleteDelivery(string id)
    {
        await _deliveryRepository.DeleteDelivery(id);
    }

    public async Task<IEnumerable<Delivery>> GetAllDeliveries()
    {
        var result = await _deliveryRepository.GetAll();
        return result;
    }
    
    public async Task CheckAndExpireDeliveries()
    {
        var currentTime = DateTime.UtcNow;

        var expiredDeliveries = await _deliveryRepository.GetExpiredDeliveries(currentTime);

        var expiredDeliveriesDtoList = expiredDeliveries.Select(delivery => 
            new UpdateDeliveryDto { Id = delivery.Id, State = DeliveryState.Expired }).ToList();
        await _deliveryRepository.UpdateDeliveries(expiredDeliveriesDtoList);
    }
}