using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Entities;

namespace TT.Deliveries.Data.Dto.Repositories;

public interface IDeliveryRepository
{
    Task<string> CreateDelivery(CreateDeliveryDto createDeliveryDto);
    Task<Delivery> GetDelivery(string id);
    Task UpdateDelivery(UpdateDeliveryDto updateDeliveryDto);
    Task DeleteDelivery(string id);
    Task<IEnumerable<Delivery>> GetAll();
    Task<IEnumerable<Delivery>> GetExpiredDeliveries(DateTime currentTime);
    Task UpdateDeliveries(List<UpdateDeliveryDto> updateDeliveryDtos);
}