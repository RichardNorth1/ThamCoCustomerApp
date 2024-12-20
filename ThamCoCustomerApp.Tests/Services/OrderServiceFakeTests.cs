using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Services.OrderService;

namespace ThamCoCustomerApp.Tests.Services
{
    [TestFixture]
    public class OrderServiceFakeTests
    {
        private OrderServiceFake _orderService;

        [SetUp]
        public void Setup()
        {
            _orderService = new OrderServiceFake();
        }

        [Test]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            var response = await _orderService.GetOrder(999);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task GetOrder_ReturnsOrder_WhenOrderExists()
        {
            var response = await _orderService.GetOrder(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var contentString = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<OrderDto>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.OrderId);
        }

        [Test]
        public async Task GetOrders_ReturnsOrders_WhenCustomerIdExists()
        {
            var customerId = "1";

            var response = await _orderService.GetOrders(customerId);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var contentString = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.IsNotNull(orders);
            Assert.IsTrue(orders.Count > 0);
            Assert.IsTrue(orders.TrueForAll(o => o.CustomerId == customerId));
        }

        [Test]
        public async Task GetOrders_UpdatesPredefinedOrders_WhenCustomerIdDoesNotExist()
        {
            var customerId = "newCustomer";

            var response = await _orderService.GetOrders(customerId);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var contentString = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.IsNotNull(orders);
            Assert.IsTrue(orders.Count > 0);
            Assert.IsTrue(orders.TrueForAll(o => o.CustomerId == customerId));
        }
    }
}
