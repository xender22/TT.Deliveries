using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Entities;

namespace TT.Services;

public interface IDeliveryService
{
    Task<string> CreateDelivery(CreateDeliveryDto createDeliveryDto);
    Task UpdateDelivery(UpdateDeliveryDto updateDeliveryDto);
    Task<Delivery> GetDelivery(string id);
    Task DeleteDelivery(string id);
    Task<IEnumerable<Delivery>> GetAllDeliveries();
    Task CheckAndExpireDeliveries();
}