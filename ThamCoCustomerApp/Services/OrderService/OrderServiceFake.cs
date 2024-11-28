using System.Net;
using System.Text.Json;
using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Services.OrderService
{
    public class OrderServiceFake : IOrderService
    {
        private readonly List<OrderDto> _orders;

        public OrderServiceFake()
        {
            _orders = new List<OrderDto>
            {
                new OrderDto
                {
                    OrderId = 1,
                    CustomerId = null, // Initially null
                    OrderStatus = "Pending",
                    Products = new List<CompanyWithProductDto>
                    {
                        new CompanyWithProductDto
                        {
                            ProductId = 1,
                            CompanyId = 1,
                            Name = "Product 1",
                            Brand = "Brand 1",
                            Description = "Description 1",
                            Price = 10.00,
                            ImageUrl = "https://via.placeholder.com/150",
                            StockLevel = 0
                        },
                        new CompanyWithProductDto
                        {
                            ProductId = 2,
                            CompanyId = 1,
                            Name = "Product 2",
                            Brand = "Brand 2",
                            Description = "Description 2",
                            Price = 15.00,
                            ImageUrl = "https://via.placeholder.com/150",
                            StockLevel = 0
                        }
                    }
                },
                new OrderDto
                {
                    OrderId = 2,
                    CustomerId = null, // Initially null
                    OrderStatus = "Delivered",
                    Products = new List<CompanyWithProductDto>
                    {
                        new CompanyWithProductDto
                        {
                            ProductId = 3,
                            CompanyId = 1,
                            Name = "Product 3",
                            Brand = "Brand 3",
                            Description = "Description 3",
                            Price = 20.00,
                            ImageUrl = "https://via.placeholder.com/150",
                            StockLevel = 0
                        },
                        new CompanyWithProductDto
                        {
                            ProductId = 4,
                            CompanyId = 1,
                            Name = "Product 4",
                            Brand = "Brand 4",
                            Description = "Description 4",
                            Price = 25.00,
                            ImageUrl = "https://via.placeholder.com/150",
                            StockLevel = 0
                        }
                    }
                }
            };
        }

        public Task<HttpResponseMessage> GetOrder(int orderId)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(order))
            });
        }

        public Task<HttpResponseMessage> GetOrders(string customerId)
        {
            var orders = _orders.Where(o => o.CustomerId == customerId).ToList();
            if (!orders.Any())
            {
                // Update predefined orders with the customer ID using LINQ
                _orders.ForEach(o => o.CustomerId = customerId);
                orders = _orders;
            }
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(orders))
            });
        }
    }
}
