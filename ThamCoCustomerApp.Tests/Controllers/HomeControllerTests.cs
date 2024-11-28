using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
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
    public class HomeControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private IMapper _mapper;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CompanyWithProductDto, ProductViewModel>();
            });
            _mapper = config.CreateMapper();

            _controller = new HomeController(_mockProductService.Object, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProduct(It.IsAny<int>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsViewResult_WithProductViewModel()
        {
            // Arrange
            var productDto = new CompanyWithProductDto { ProductId = 1, Name = "Test Product" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(productDto))
            };
            _mockProductService.Setup(s => s.GetProduct(It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<ProductViewModel>(viewResult.Model);
            var model = viewResult.Model as ProductViewModel;
            Assert.AreEqual(productDto.ProductId, model.ProductId);
            Assert.AreEqual(productDto.Name, model.Name);
        }

        [Test]
        public async Task GetProducts_ReturnsJsonResult_WithProductViewModels()
        {
            // Arrange
            var productDtos = new List<CompanyWithProductDto>
            {
                new CompanyWithProductDto { ProductId = 1, Name = "Test Product 1" },
                new CompanyWithProductDto { ProductId = 2, Name = "Test Product 2" }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(productDtos))
            };
            _mockProductService.Setup(s => s.GetProducts()).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.GetProducts(null);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var productViewModels = jsonResult.Value as List<ProductViewModel>;
            Assert.IsNotNull(productViewModels);
            Assert.AreEqual(productDtos.Count, productViewModels.Count);
        }

        [Test]
        public async Task GetProducts_FiltersProducts_WhenSearchStringProvided()
        {
            // Arrange
            var productDtos = new List<CompanyWithProductDto>
            {
                new CompanyWithProductDto { ProductId = 1, Name = "Test Product 1", Brand = "Brand A", Description = "Description A" },
                new CompanyWithProductDto { ProductId = 2, Name = "Another Product", Brand = "Brand B", Description = "Description B" }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(productDtos))
            };
            _mockProductService.Setup(s => s.GetProducts()).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.GetProducts("Test");

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var productViewModels = jsonResult.Value as List<ProductViewModel>;
            Assert.IsNotNull(productViewModels);
            Assert.AreEqual(1, productViewModels.Count);
            Assert.AreEqual("Test Product 1", productViewModels[0].Name);
        }
    }
}
