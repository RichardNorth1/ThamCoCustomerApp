using System.Net;
using System.Text.Json;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;

namespace ThamCoCustomerApp.Services.BasketService
{
    public class BasketServiceFake : IBasketService
    {
        private readonly List<BasketDto> _baskets;

        public BasketServiceFake()
        {
            _baskets = new List<BasketDto>();
        }

        public Task<HttpResponseMessage> AddToBasket(string customerId, CompanyWithProductDto product)
        {
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase));
            if (basket != null)
            {
                basket.Products.Add(product);

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(basket))
                });
            }
            else
            {
                var newBasket = new BasketDto
                {
                    CustomerId = customerId,
                    Products = new List<CompanyWithProductDto> { product }
                };
                _baskets.Add(newBasket);

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(newBasket))
                });
            }
        }

        public Task<HttpResponseMessage> ClearBasket(string customerId)
        {
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase));
            if (basket != null)
            {
                basket.Products.Clear();

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(basket))
                });
            }
            else
            {
                var newBasket = new BasketDto
                {
                    CustomerId = customerId,
                    Products = new List<CompanyWithProductDto>()
                };
                _baskets.Add(newBasket);
                _baskets.FirstOrDefault(b => b.CustomerId.Equals(customerId, StringComparison.OrdinalIgnoreCase)).Products.Clear();

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(newBasket))
                });
            }
        }

        public Task<HttpResponseMessage> GetBasket(string customerId)
        {
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase));
            if (basket != null)
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(basket))
                });
            }
            else
            {
                var newBasket = new BasketDto
                {
                    CustomerId = customerId,
                    Products = new List<CompanyWithProductDto>()
                };
                _baskets.Add(newBasket);

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(newBasket))
                });
            }
        }

        public Task<HttpResponseMessage> RemoveFromBasket(string customerId, CompanyWithProductDto product)
        {
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase));
            if (basket != null)
            {
                var productToRemove = basket.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (productToRemove != null)
                {
                    basket.Products.Remove(productToRemove);
                }

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(basket))
                });
            }
            else
            {
                var newBasket = new BasketDto
                {
                    CustomerId = customerId,
                    Products = new List<CompanyWithProductDto>()
                };
                _baskets.Add(newBasket);

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(newBasket))
                });
            }
        }
    }
}
