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
using ThamCoCustomerApp.Services.BasketService;

namespace ThamCoCustomerApp.Tests.Controllers
{
    [TestFixture]
    public class BasketControllerTests
    {
        private Mock<IBasketService> _mockBasketService;
        private BasketController _controller;

        [SetUp]
        public void SetUp()
        {

            _mockBasketService = new Mock<IBasketService>();
            _controller = new BasketController(_mockBasketService.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithBasketViewModel_WhenBasketExists()
        {
            // Arrange
            var basket = new BasketDto
            {
                Products = new List<CompanyWithProductDto>
                {
                    new CompanyWithProductDto { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1", Price = 10.0, StockLevel = 1, CompanyId = 1, ImageUrl = "" },
                    new CompanyWithProductDto { ProductId = 2, Name = "Product2", Brand = "Brand2", Description = "Description2", Price = 20.0, StockLevel = 1, CompanyId = 1, ImageUrl = ""}
                }
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(basket))
            };

            _mockBasketService.Setup(s => s.GetBasket("user123")).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as BasketViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.That(model.Products.Count, Is.EqualTo(2));
            Assert.That(model.Products[0].Name, Is.EqualTo("Product1"));
            Assert.That(model.Products[1].Name, Is.EqualTo("Product2"));
        }

        [Test]
        public async Task Index_ReturnsViewWithEmptyBasketViewModel_WhenBasketDoesNotExist()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockBasketService.Setup(s => s.GetBasket("user123")).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as BasketViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.That(model.Products.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task AddToBasket_RedirectsToIndex_WhenProductIsAddedSuccessfully()
        {
            // Arrange
            var productViewModel = new ProductViewModel
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "Product1",
                Brand = "Brand1",
                Description = "Description1",
                Price = 10.0
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockBasketService.Setup(s => s.AddToBasket("user123", It.IsAny<CompanyWithProductDto>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.AddToBasket(productViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task RemoveFromBasket_RedirectsToIndex_WhenProductIsRemovedSuccessfully()
        {
            // Arrange
            var productViewModel = new ProductViewModel
            {
                ProductId = 1,
                CompanyId = 1,
                Name = "Product1",
                Brand = "Brand1",
                Description = "Description1",
                Price = 10.0
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockBasketService.Setup(s => s.RemoveFromBasket("user123", It.IsAny<CompanyWithProductDto>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.RemoveFromBasket(productViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task ClearBasket_RedirectsToIndex_WhenBasketIsClearedSuccessfully()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockBasketService.Setup(s => s.ClearBasket("user123")).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.ClearBasket() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }


    }
}
