using TT.Deliveries.Data.Dto.Entities;
using TT.Deliveries.Data.Dto.Enums;

namespace TT.Deliveries.Data.Dto.Dtos
{
    public class CreateDeliveryDto
    {
        public DeliveryState State { get; set; }
        public AccessWindow AccessWindow { get; set; }
        public Recipient Recipient { get; set; }
        public Order Order { get; set; }
    }
}
