using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using ThamCoCustomerApp.Controllers;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.OrderService;

namespace ThamCoCustomerApp.Tests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        private Mock<IOrderService> _mockOrderService;
        private IMapper _mapper;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _mockOrderService = new Mock<IOrderService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDto, OrderViewModel>();
            });
            _mapper = config.CreateMapper();

            _controller = new OrderController(_mockOrderService.Object, _mapper);

            // Mock the User property with claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexAsync_ReturnsNotFound_WhenOrdersNotFound()
        {
            // Arrange
            _mockOrderService.Setup(s => s.GetOrders(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            // Act
            var result = await _controller.IndexAsync();

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task IndexAsync_ReturnsViewResult_WithOrderViewModels()
        {
            // Arrange
            var orderDtos = new List<OrderDto>
            {
                new OrderDto { OrderId = 1, CustomerId = "1", OrderStatus = "Pending" },
                new OrderDto { OrderId = 2, CustomerId = "1", OrderStatus = "Delivered" }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(orderDtos))
            };
            _mockOrderService.Setup(s => s.GetOrders(It.IsAny<string>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.IndexAsync();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<IEnumerable<OrderViewModel>>(viewResult.Model);
            var model = viewResult.Model as IEnumerable<OrderViewModel>;
            Assert.AreEqual(orderDtos.Count, model.Count());
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenOrderNotFound()
        {
            // Arrange
            _mockOrderService.Setup(s => s.GetOrder(It.IsAny<int>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsViewResult_WithOrderViewModel()
        {
            // Arrange
            var orderDto = new OrderDto { OrderId = 1, CustomerId = "1", OrderStatus = "Pending" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(orderDto))
            };
            _mockOrderService.Setup(s => s.GetOrder(It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<OrderViewModel>(viewResult.Model);
            var model = viewResult.Model as OrderViewModel;
            Assert.AreEqual(orderDto.OrderId, model.OrderId);
            Assert.AreEqual(orderDto.CustomerId, model.CustomerId);
            Assert.AreEqual(orderDto.OrderStatus, model.OrderStatus);
        }
    }
}

