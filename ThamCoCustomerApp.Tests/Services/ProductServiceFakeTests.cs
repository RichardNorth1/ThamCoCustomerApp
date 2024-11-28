using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Services.Product;

namespace ThamCoCustomerApp.Tests.Services
{
    [TestFixture]
    public class ProductServiceFakeTests
    {
        private ProductServiceFake _productServiceFake;

        [SetUp]
        public void SetUp()
        {
            _productServiceFake = new ProductServiceFake();
        }

        [Test]
        public async Task GetProduct_ShouldReturnProduct_WhenProductIdExists()
        {
            // Arrange
            var productId = 1;

            // Act
            var response = await _productServiceFake.GetProduct(productId);
            var product = JsonSerializer.Deserialize<CompanyWithProductDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.ProductId);
        }

        [Test]
        public async Task GetProduct_ShouldReturnNull_WhenProductIdDoesNotExist()
        {
            // Arrange
            var productId = 999;

            // Act
            var response = await _productServiceFake.GetProduct(productId);
            var product = JsonSerializer.Deserialize<CompanyWithProductDto>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNull(product);
        }

        [Test]
        public async Task GetProducts_ShouldReturnAllProducts()
        {
            // Act
            var response = await _productServiceFake.GetProducts();
            var products = JsonSerializer.Deserialize<IEnumerable<CompanyWithProductDto>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(products);
            Assert.AreEqual(5, products.Count());
        }

        [Test]
        public async Task GetProducts_ShouldReturnCorrectProductDetails()
        {
            // Act
            var response = await _productServiceFake.GetProducts();
            var products = JsonSerializer.Deserialize<IEnumerable<CompanyWithProductDto>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(products);

            var product = products.FirstOrDefault(p => p.ProductId == 1);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.ProductId);
            Assert.AreEqual("Product 1", product.Name);
            Assert.AreEqual("Brand 1", product.Brand);
            Assert.AreEqual("Description 1", product.Description);
            Assert.AreEqual(10, product.Price);
            Assert.AreEqual("data:image/jpeg;base64,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", product.ImageUrl);
            Assert.AreEqual(10, product.StockLevel);
        }
    }
}
