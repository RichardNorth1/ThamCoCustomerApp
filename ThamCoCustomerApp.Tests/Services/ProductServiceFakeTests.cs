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
            Assert.AreEqual("imageUrl", product.ImageUrl);
            Assert.AreEqual(10, product.StockLevel);
        }
    }
}
