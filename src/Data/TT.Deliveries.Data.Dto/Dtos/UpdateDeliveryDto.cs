using TT.Deliveries.Data.Dto.Entities;
using TT.Deliveries.Data.Dto.Enums;

namespace TT.Deliveries.Data.Dto.Dtos
{
    public class UpdateDeliveryDto
    {
        public string Id { get; set; }
        public DeliveryState State { get; set; }
    }
}
