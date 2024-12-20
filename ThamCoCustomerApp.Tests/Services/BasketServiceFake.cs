using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Services.BasketService;

namespace ThamCoCustomerApp.Tests.Services
{
    [TestFixture]
    public class BasketServiceFakeTests
    {
        private BasketServiceFake _basketServiceFake;

        [SetUp]
        public void SetUp()
        {
            _basketServiceFake = new BasketServiceFake();
        }

        [Test]
        public async Task AddToBasket_ShouldAddProduct_WhenCustomerHasNoBasket()
        {
            // Arrange
            var customerId = "123";
            var product = new CompanyWithProductDto 
            { 
                ProductId = 1,
                CompanyId = 1,
                Name = "test product",
                Brand = "Test Brand",
                Description = "Test Description",
                Price = 10,
                ImageUrl = "Test Url",
                StockLevel = 1
            
            };

            var response = await _basketServiceFake.AddToBasket(customerId, product);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product.ProductId, basket.Products.First().ProductId);
        }

        [Test]
        public async Task AddToBasket_ShouldAddProduct_WhenCustomerHasBasket()
        {
            var customerId = "123";
            var product1 =  new CompanyWithProductDto
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "test product",
                Brand = "Test Brand",
                Description = "Test Description",
                Price = 10,
                ImageUrl = "Test Url",
                StockLevel = 1

            };
            var product2 = new CompanyWithProductDto
            {
                ProductId = 2,
                CompanyId = 1,
                Name = "test product 2",
                Brand = "Test Brand 2",
                Description = "Test Description 2",
                Price = 20,
                ImageUrl = "Test Url 2",
                StockLevel = 2

            };

            await _basketServiceFake.AddToBasket(customerId, product1);

            var response = await _basketServiceFake.AddToBasket(customerId, product2);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(2, basket.Products.Count);
            Assert.IsTrue(basket.Products.Any(p => p.ProductId == product1.ProductId));
            Assert.IsTrue(basket.Products.Any(p => p.ProductId == product2.ProductId));
        }

        [Test]
        public async Task ClearBasket_ShouldClearProducts_WhenCustomerHasBasket()
        {
            var customerId = "123";
            var product = new CompanyWithProductDto
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "test product",
                Brand = "Test Brand",
                Description = "Test Description",
                Price = 10,
                ImageUrl = "Test Url",
                StockLevel = 1

            };

            await _basketServiceFake.AddToBasket(customerId, product);

            var response = await _basketServiceFake.ClearBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(0, basket.Products.Count);
        }

        [Test]
        public async Task ClearBasket_ShouldCreateEmptyBasket_WhenCustomerHasNoBasket()
        {
            var customerId = "123";

            var response = await _basketServiceFake.ClearBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(0, basket.Products.Count);
        }

        [Test]
        public async Task GetBasket_ShouldReturnBasket_WhenCustomerHasBasket()
        {
            var customerId = "123";
            var product = new CompanyWithProductDto
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "test product",
                Brand = "Test Brand",
                Description = "Test Description",
                Price = 10,
                ImageUrl = "Test Url",
                StockLevel = 1

            };

            await _basketServiceFake.AddToBasket(customerId, product);

            var response = await _basketServiceFake.GetBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product.ProductId, basket.Products.First().ProductId);
        }

        [Test]
        public async Task GetBasket_ShouldCreateEmptyBasket_WhenCustomerHasNoBasket()
        {
            var customerId = "123";

            var response = await _basketServiceFake.GetBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(0, basket.Products.Count);
        }

        [Test]
        public async Task RemoveFromBasket_ShouldRemoveProduct_WhenProductExistsInBasket()
        {
            var customerId = "123";
            var product1 = new CompanyWithProductDto
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "test product",
                Brand = "Test Brand",
                Description = "Test Description",
                Price = 10,
                ImageUrl = "Test Url",
                StockLevel = 1

            };
            var product2  = new CompanyWithProductDto
            {
                ProductId = 2,
                CompanyId = 2,
                Name = "test product 2",
                Brand = "Test Brand 2",
                Description = "Test Description 2",
                Price = 20,
                ImageUrl = "Test Url 2",
                StockLevel = 2

            };

            await _basketServiceFake.AddToBasket(customerId, product1);
            await _basketServiceFake.AddToBasket(customerId, product2);

            var response = await _basketServiceFake.RemoveFromBasket(customerId, product1);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product2.ProductId, basket.Products.First().ProductId);
        }

        [Test]
        public async Task RemoveFromBasket_ShouldDoNothing_WhenProductDoesNotExistInBasket()
        {
            var customerId = "123";
            var product1 = new CompanyWithProductDto
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "test product",
                Brand = "Test Brand",
                Description = "Test Description",
                Price = 10,
                ImageUrl = "Test Url",
                StockLevel = 1

            };
            var product2 = new CompanyWithProductDto
            {
                ProductId = 2,
                CompanyId = 2,
                Name = "test product 2",
                Brand = "Test Brand 2",
                Description = "Test Description 2",
                Price = 20,
                ImageUrl = "Test Url 2",
                StockLevel = 2

            };

            await _basketServiceFake.AddToBasket(customerId, product1);

            var response = await _basketServiceFake.RemoveFromBasket(customerId, product2);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product1.ProductId, basket.Products.First().ProductId);
        }
    }
}
