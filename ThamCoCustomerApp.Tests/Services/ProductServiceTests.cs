using Moq;
using Moq.Protected;
using NUnit.Framework;
using Polly.CircuitBreaker;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Services.Product;
using ThamCoCustomerApp.Services.Token;

namespace ThamCoCustomerApp.Tests.Services
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<ITokenService> _tokenServiceMock;
        private HttpClient _httpClient;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _tokenServiceMock = new Mock<ITokenService>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _productService = new ProductService(_httpClient, _tokenServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task GetProduct_ShouldReturnProduct_WhenProductIdExists()
        {
            // Arrange
            var productId = 1;
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"productId\":1,\"name\":\"Product 1\"}")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri($"http://localhost/api/products/{productId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            _tokenServiceMock.Setup(ts => ts.GetToken()).ReturnsAsync(new TokenDto { AccessToken = "fake-token" });

            // Act
            var response = await _productService.GetProduct(productId);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content.Contains("\"productId\":1"));
        }

        [Test]
        public async Task GetProducts_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[{\"productId\":1,\"name\":\"Product 1\"},{\"productId\":2,\"name\":\"Product 2\"}]")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri("http://localhost/api/products")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            _tokenServiceMock.Setup(ts => ts.GetToken()).ReturnsAsync(new TokenDto { AccessToken = "fake-token" });

            // Act
            var response = await _productService.GetProducts();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content.Contains("\"productId\":1"));
            Assert.That(content.Contains("\"productId\":2"));
        }

        [Test]
        public void GetProduct_ShouldThrowBrokenCircuitException_WhenCircuitIsOpen()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new BrokenCircuitException());

            _tokenServiceMock.Setup(ts => ts.GetToken()).ReturnsAsync(new TokenDto { AccessToken = "fake-token" });

            // Act & Assert
            Assert.ThrowsAsync<BrokenCircuitException>(async () => await _productService.GetProduct(1));
        }

        [Test]
        public void GetProducts_ShouldThrowBrokenCircuitException_WhenCircuitIsOpen()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new BrokenCircuitException());

            _tokenServiceMock.Setup(ts => ts.GetToken()).ReturnsAsync(new TokenDto { AccessToken = "fake-token" });

            // Act & Assert
            Assert.ThrowsAsync<BrokenCircuitException>(async () => await _productService.GetProducts());
        }
    }
}
