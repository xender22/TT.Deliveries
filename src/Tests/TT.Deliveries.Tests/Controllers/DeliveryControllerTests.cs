using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using TT.Deliveries.Data.Dto.Dtos;
using TT.Deliveries.Data.Dto.Entities;
using TT.Deliveries.Data.Dto.Enums;
using TT.Deliveries.Web.Api.Controllers;
using TT.Services;
using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TT.Deliveries.Tests.Controllers
{


    [TestFixture]
    public class DeliveriesControllerTests
    {
        private Mock<IDeliveryService> _deliveryServiceMock;
        private DeliveriesController _controller;

        [SetUp]
        public void Setup()
        {
            _deliveryServiceMock = new Mock<IDeliveryService>();
            _controller = new DeliveriesController(_deliveryServiceMock.Object);
        }

        [Test]
        public async Task GetDeliveries_ReturnsOkResult()
        {
            // Arrange
            var deliveries = new List<Delivery>(); // Add sample deliveries as needed
            _deliveryServiceMock.Setup(s => s.GetAllDeliveries()).ReturnsAsync(deliveries);

            // Act
            var result = await _controller.GetDeliveries();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedDeliveries = okResult.Value as IEnumerable<Delivery>;
            Assert.NotNull(returnedDeliveries);
            Assert.AreEqual(deliveries, returnedDeliveries);
        }

        [Test]
        public async Task GetDelivery_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var delivery = new Delivery { Id = id }; // Create a sample delivery
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(delivery);

            // Act
            var result = await _controller.GetDelivery(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedDelivery = okResult.Value as Delivery;
            Assert.NotNull(returnedDelivery);
            Assert.AreEqual(delivery, returnedDelivery);
        }

        [Test]
        public async Task GetDelivery_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var delivery = new Delivery(); // Create a sample delivery with null Id
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(delivery);

            // Act
            var result = await _controller.GetDelivery(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task CreateDelivery_WithValidDto_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var createDeliveryDto = new CreateDeliveryDto(); // Create a sample createDeliveryDto
            var createdId = ObjectId.GenerateNewId().ToString();
            _deliveryServiceMock.Setup(s => s.CreateDelivery(createDeliveryDto)).ReturnsAsync(createdId);

            // Act
            var result = await _controller.CreateDelivery(createDeliveryDto);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.AreEqual(nameof(DeliveriesController.GetDelivery), createdAtActionResult.ActionName);
            Assert.AreEqual(createdId, createdAtActionResult.RouteValues["id"]);
            Assert.AreEqual(createdId, createdAtActionResult.Value);
        }

        [Test]
        public async Task UpdateDelivery_WithValidDto_ReturnsNoContentResult()
        {
            // Arrange
            var updateDeliveryDto = new UpdateDeliveryDto(); // Create a sample updateDeliveryDto
            var existingDelivery = new Delivery { Id = ObjectId.GenerateNewId().ToString() }; // Create a sample existing delivery
            _deliveryServiceMock.Setup(s => s.GetDelivery(updateDeliveryDto.Id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.UpdateDelivery(updateDeliveryDto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateDelivery_WithInvalidDto_ReturnsNotFoundResult()
        {
            // Arrange
            var updateDeliveryDto = new UpdateDeliveryDto { Id = ObjectId.GenerateNewId().ToString() }; // Create a sample updateDeliveryDto with non-existing Id
            var existingDelivery = new Delivery(); // Create a sample existing delivery with null Id
            _deliveryServiceMock.Setup(s => s.GetDelivery(updateDeliveryDto.Id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.UpdateDelivery(updateDeliveryDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteDelivery_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var existingDelivery = new Delivery { Id = id }; // Create a sample existing delivery
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.DeleteDelivery(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteDelivery_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var existingDelivery = new Delivery(); // Create a sample existing delivery with null Id
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.DeleteDelivery(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task ApproveDelivery_WithExpiredDelivery_ReturnsBadRequestResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var currentTime = DateTime.UtcNow;
            var existingDelivery = new Delivery
            {
                Id = id,
                AccessWindow = new AccessWindow
                {
                    EndTime = currentTime.AddMinutes(-1) // Set the end time in the past to simulate an expired delivery
                }
            };
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.ApproveDelivery(id);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task ApproveDelivery_WithValidDelivery_ReturnsNoContentResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var currentTime = DateTime.UtcNow;
            var existingDelivery = new Delivery
            {
                Id = id,
                AccessWindow = new AccessWindow
                {
                    EndTime = currentTime.AddMinutes(10) // Set the end time in the future to simulate a valid delivery
                }
            };
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);
            _deliveryServiceMock.Setup(s => s.UpdateDelivery(It.IsAny<UpdateDeliveryDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ApproveDelivery(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task CompleteDelivery_WithInvalidState_ReturnsBadRequestResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var existingDelivery = new Delivery
            {
                Id = id,
                State = DeliveryState.Created // Set the delivery state to 'Created' to simulate an invalid state
            };
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.CompleteDelivery(id);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CompleteDelivery_WithValidState_ReturnsNoContentResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var existingDelivery = new Delivery
            {
                Id = id,
                State = DeliveryState.Approved // Set the delivery state to 'Approved' to simulate a valid state
            };
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);
            _deliveryServiceMock.Setup(s => s.UpdateDelivery(It.IsAny<UpdateDeliveryDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CompleteDelivery(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task CancelPendingDelivery_WithInvalidState_ReturnsBadRequestResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var existingDelivery = new Delivery
            {
                Id = id,
                State = DeliveryState.Expired // Set the delivery state to 'Approved' to simulate an invalid state
            };
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);

            // Act
            var result = await _controller.CancelPendingDelivery(id);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CancelPendingDelivery_WithValidState_ReturnsNoContentResult()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var existingDelivery = new Delivery
            {
                Id = id,
                State = DeliveryState.Created // Set the delivery state to 'Created' to simulate a valid state
            };
            _deliveryServiceMock.Setup(s => s.GetDelivery(id)).ReturnsAsync(existingDelivery);
            _deliveryServiceMock.Setup(s => s.UpdateDelivery(It.IsAny<UpdateDeliveryDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelPendingDelivery(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
