using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Entities;
using TT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TT.Deliveries.Data.Dto.Enums;

namespace TT.Deliveries.Web.Api.Controllers
{
    [Route("deliveries")]
    [ApiController]
    [Produces("application/json")]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveriesController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpGet]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveries()
        {
            var deliveries = await _deliveryService.GetAllDeliveries();
            return Ok(deliveries);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Partner")]
        public async Task<ActionResult<Delivery>> GetDelivery(string id)
        {
            var delivery = await _deliveryService.GetDelivery(id);

            if (delivery.Id == null)
                return NotFound();

            return Ok(delivery);
        }

        [HttpPost]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<Delivery>> CreateDelivery(CreateDeliveryDto createDeliveryDto)
        {
            var createdDeliveryId = await _deliveryService.CreateDelivery(createDeliveryDto);
            return CreatedAtAction(nameof(GetDelivery), new { id = createdDeliveryId }, createdDeliveryId);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User, Partner")]
        public async Task<IActionResult> UpdateDelivery(UpdateDeliveryDto updateDeliveryDto)
        {
            var existingDelivery = await _deliveryService.GetDelivery(updateDeliveryDto.Id);

            if (existingDelivery.Id == null)
                return NotFound();
            
            await _deliveryService.UpdateDelivery(updateDeliveryDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> DeleteDelivery(string id)
        {
            var existingDelivery = await _deliveryService.GetDelivery(id);

            if (existingDelivery.Id == null)
                return NotFound();

            await _deliveryService.DeleteDelivery(id);

            return NoContent();
        }

        [HttpPut("approve/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ApproveDelivery(string id)
        {
            var currentTime = DateTime.UtcNow;
            
            var existingDelivery = await _deliveryService.GetDelivery(id);
            if (existingDelivery.Id == null) return NotFound();
        
            if (existingDelivery.AccessWindow.EndTime < currentTime)
                return BadRequest("Cannot approve an expired delivery");
        
            var approveStatus = new UpdateDeliveryDto
            {
                Id = existingDelivery.Id,
                State = DeliveryState.Approved
            };
        
            await _deliveryService.UpdateDelivery(approveStatus);
        
            return NoContent();
        }
        
        [HttpPut("complete/{id}")]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> CompleteDelivery(string id)
        {
            var existingDelivery = await _deliveryService.GetDelivery(id);
            if (existingDelivery.Id == null) return NotFound();
        
            if (existingDelivery.State != DeliveryState.Approved)
                return BadRequest("Delivery cannot be approved: Invalid state");
            
            var approveStatus = new UpdateDeliveryDto
            {
                Id = existingDelivery.Id,
                State = DeliveryState.Completed
            };
        
            await _deliveryService.UpdateDelivery(approveStatus);
        
            return NoContent();
        }
        
        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "User, Partner")]
        public async Task<IActionResult> CancelPendingDelivery(string id)
        {
            var existingDelivery = await _deliveryService.GetDelivery(id);
            if (existingDelivery.Id == null) return NotFound();
        
            if (existingDelivery.State is not (DeliveryState.Approved or DeliveryState.Created))
                return BadRequest("Delivery cannot be approved: Invalid state");
            
            var approveStatus = new UpdateDeliveryDto
            {
                Id = existingDelivery.Id,
                State = DeliveryState.Cancelled
            };
        
            await _deliveryService.UpdateDelivery(approveStatus);
        
            return NoContent();
        }

    }
}
