using Moq;
using NUnit.Framework;
using ThamCoCustomerApp.Controllers;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private Mock<IAccountService> _mockAccountService;
        private AccountController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountController(_mockAccountService.Object);

            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "auth0|123"),
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim("picture", "https://example.com/picture.jpg")
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            httpContext.User = new ClaimsPrincipal(identity);

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
        public async Task ProfileAsync_ReturnsViewModelWithCustomerDetails()
        {
            // Arrange
            var customer = new CustomerAccountDto
            {
                Surname = "Doe",
                Forename = "John",
                Telephone = "123456789",
                StreetAddress = "123 Main St",
                City = "Springfield",
                County = "Illinois",
                PostalCode = "12345",
                Balance = 100.0
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(customer))
            };

            _mockAccountService
                .Setup(s => s.GetAccount(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ProfileAsync() as ViewResult;
            var model = result?.Model as UserProfileViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual("John", model.Forename);
            Assert.AreEqual("Doe", model.Surname);
            Assert.AreEqual("testuser@example.com", model.EmailAddress);
        }


        [Test]
        public async Task UpdateProfileAsync_ValidModel_UpdatesAccount()
        {
            // Arrange
            var model = new UserProfileViewModel
            {
                Forename = "Jane",
                Surname = "Doe",
                EmailAddress = "jane.doe@example.com",
                Telephone = "987654321",
                StreetAddress = "456 Another St",
                City = "Metropolis",
                County = "State",
                PostalCode = "67890"
            };

            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            _mockAccountService
                .Setup(s => s.UpdateAccount(It.IsAny<CustomerAccountDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateProfileAsync(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Profile", result.ActionName);
        }

        [Test]
        public void AccessDenied_ReturnsView()
        {
            // Act
            var result = _controller.AccessDenied() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }


    }
}

