using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ThamCoCustomerApp.Controllers;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.Product;

namespace ThamCoCustomerApp.Tests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private Mock<IMapper> _mockMapper;
        private ProductsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockProductService = new Mock<IProductService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ProductsController(_mockProductService.Object, _mockMapper.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithProductViewModels_WhenProductsExist()
        {
            // Arrange
            var products = new List<CompanyWithProductDto>
            {
                new CompanyWithProductDto { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" },
                new CompanyWithProductDto { ProductId = 2, Name = "Product2", Brand = "Brand2", Description = "Description2" }
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(products))
            };

            _mockProductService.Setup(s => s.GetProducts()).ReturnsAsync(responseMessage);
            _mockMapper.Setup(m => m.Map<List<ProductViewModel>>(It.IsAny<List<CompanyWithProductDto>>()))
                       .Returns(new List<ProductViewModel>
                       {
                           new ProductViewModel { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" },
                           new ProductViewModel { ProductId = 2, Name = "Product2", Brand = "Brand2", Description = "Description2" }
                       });

            // Act
            var result = await _controller.Index(null) as ViewResult;
            var model = result?.Model as List<ProductViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Product1", model[0].Name);
            Assert.AreEqual("Product2", model[1].Name);
        }

        [Test]
        public async Task Index_ReturnsFilteredProducts_WhenSearchStringIsProvided()
        {
            // Arrange
            var products = new List<CompanyWithProductDto>
            {
                new CompanyWithProductDto { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" },
                new CompanyWithProductDto { ProductId = 2, Name = "Product2", Brand = "Brand2", Description = "Description2" }
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(products))
            };

            _mockProductService.Setup(s => s.GetProducts()).ReturnsAsync(responseMessage);
            _mockMapper.Setup(m => m.Map<List<ProductViewModel>>(It.IsAny<List<CompanyWithProductDto>>()))
                       .Returns(new List<ProductViewModel>
                       {
                           new ProductViewModel { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" },
                           new ProductViewModel { ProductId = 2, Name = "Product2", Brand = "Brand2", Description = "Description2" }
                       });

            // Act
            var result = await _controller.Index("Product1") as ViewResult;
            var model = result?.Model as List<ProductViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Product1", model[0].Name);
        }

        [Test]
        public async Task Details_ReturnsViewWithProductViewModel_WhenProductExists()
        {
            // Arrange
            var product = new CompanyWithProductDto { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(product))
            };

            _mockProductService.Setup(s => s.GetProduct(1)).ReturnsAsync(responseMessage);
            _mockMapper.Setup(m => m.Map<ProductViewModel>(It.IsAny<CompanyWithProductDto>()))
                       .Returns(new ProductViewModel { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" });

            // Act
            var result = await _controller.Details(1) as ViewResult;
            var model = result?.Model as ProductViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual("Product1", model.Name);
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockProductService.Setup(s => s.GetProduct(1)).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetProducts_ReturnsOkWithProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<CompanyWithProductDto>
            {
                new CompanyWithProductDto { ProductId = 1, Name = "Product1", Brand = "Brand1", Description = "Description1" },
                new CompanyWithProductDto { ProductId = 2, Name = "Product2", Brand = "Brand2", Description = "Description2" }
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(products))
            };

            _mockProductService.Setup(s => s.GetProducts()).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.GetProducts("");
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null");

            var returnedProducts = okResult.Value as IEnumerable<CompanyWithProductDto>;
            Assert.IsNotNull(returnedProducts, "Expected returned products but got null");

            // Assert
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(returnedProducts.Count(), Is.EqualTo(2));
        }


        [Test]
        public void GetProducts_ThrowsException_WhenServiceFails()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = "Internal Server Error"
            };

            _mockProductService.Setup(s => s.GetProducts()).ReturnsAsync(responseMessage);

            // Act & Assert
            var ex = Assert.ThrowsAsync<HttpRequestException>(async () => await _controller.GetProducts(""));
            Assert.AreEqual("Response status code does not indicate success: 500 (Internal Server Error).", ex.Message);
        }


    }
}
