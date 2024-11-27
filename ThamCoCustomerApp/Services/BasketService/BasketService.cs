

using System.Net;
using System.Text.Json;
using ThamCoCustomerApp.Data;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;

namespace ThamCoCustomerApp.Services.BasketService
{
    public class BasketService : IBasketService
    {

        private readonly List<BasketDto> _baskets;

        public BasketService()
        {
            _baskets = new List<BasketDto>();
        }

        public Task<HttpResponseMessage> AddToBasket(string customerId, CompanyWithProductDto product)
        {
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId));
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
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId));
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
                _baskets.FirstOrDefault(b => b.CustomerId.Equals(customerId)).Products.Clear();

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(newBasket))
                });
            
            }
        }

        public Task<HttpResponseMessage> GetBasket(string customerId)
        {

            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId));
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
            var basket = _baskets.FirstOrDefault(c => string.Equals(c.CustomerId, customerId));
            if (basket != null)
            {
                basket.Products.RemoveAll(p => p.ProductId == product.ProductId);
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
