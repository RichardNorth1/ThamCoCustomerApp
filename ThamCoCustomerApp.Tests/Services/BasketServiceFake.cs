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
            var customerId = "customer1";
            var product = new CompanyWithProductDto { ProductId = 1, Name = "Product 1" };

            // Act
            var response = await _basketServiceFake.AddToBasket(customerId, product);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product.ProductId, basket.Products.First().ProductId);
        }

        [Test]
        public async Task AddToBasket_ShouldAddProduct_WhenCustomerHasBasket()
        {
            // Arrange
            var customerId = "customer1";
            var product1 = new CompanyWithProductDto { ProductId = 1, Name = "Product 1" };
            var product2 = new CompanyWithProductDto { ProductId = 2, Name = "Product 2" };

            await _basketServiceFake.AddToBasket(customerId, product1);

            // Act
            var response = await _basketServiceFake.AddToBasket(customerId, product2);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
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
            // Arrange
            var customerId = "customer1";
            var product = new CompanyWithProductDto { ProductId = 1, Name = "Product 1" };

            await _basketServiceFake.AddToBasket(customerId, product);

            // Act
            var response = await _basketServiceFake.ClearBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(0, basket.Products.Count);
        }

        [Test]
        public async Task ClearBasket_ShouldCreateEmptyBasket_WhenCustomerHasNoBasket()
        {
            // Arrange
            var customerId = "customer1";

            // Act
            var response = await _basketServiceFake.ClearBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(0, basket.Products.Count);
        }

        [Test]
        public async Task GetBasket_ShouldReturnBasket_WhenCustomerHasBasket()
        {
            // Arrange
            var customerId = "customer1";
            var product = new CompanyWithProductDto { ProductId = 1, Name = "Product 1" };

            await _basketServiceFake.AddToBasket(customerId, product);

            // Act
            var response = await _basketServiceFake.GetBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product.ProductId, basket.Products.First().ProductId);
        }

        [Test]
        public async Task GetBasket_ShouldCreateEmptyBasket_WhenCustomerHasNoBasket()
        {
            // Arrange
            var customerId = "customer1";

            // Act
            var response = await _basketServiceFake.GetBasket(customerId);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(0, basket.Products.Count);
        }

        [Test]
        public async Task RemoveFromBasket_ShouldRemoveProduct_WhenProductExistsInBasket()
        {
            // Arrange
            var customerId = "customer1";
            var product1 = new CompanyWithProductDto { ProductId = 1, Name = "Product 1" };
            var product2 = new CompanyWithProductDto { ProductId = 2, Name = "Product 2" };

            await _basketServiceFake.AddToBasket(customerId, product1);
            await _basketServiceFake.AddToBasket(customerId, product2);

            // Act
            var response = await _basketServiceFake.RemoveFromBasket(customerId, product1);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product2.ProductId, basket.Products.First().ProductId);
        }

        [Test]
        public async Task RemoveFromBasket_ShouldDoNothing_WhenProductDoesNotExistInBasket()
        {
            // Arrange
            var customerId = "customer1";
            var product1 = new CompanyWithProductDto { ProductId = 1, Name = "Product 1" };
            var product2 = new CompanyWithProductDto { ProductId = 2, Name = "Product 2" };

            await _basketServiceFake.AddToBasket(customerId, product1);

            // Act
            var response = await _basketServiceFake.RemoveFromBasket(customerId, product2);
            var basket = JsonSerializer.Deserialize<BasketDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(basket);
            Assert.AreEqual(customerId, basket.CustomerId);
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(product1.ProductId, basket.Products.First().ProductId);
        }
    }
}
