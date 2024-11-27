using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.Account;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace ThamCoCustomerApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task Login(string returnUrl = "/Account/Profile")
        {
            var authenticationProperties = new
                LoginAuthenticationPropertiesBuilder()
                    .WithRedirectUri(returnUrl)
                    .Build();

            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        [Authorize]
        public async Task Logout()
        {
            var authenticationProperties = new
                LogoutAuthenticationPropertiesBuilder()
                    .WithRedirectUri(Url.Action("Index", "Home"))
                    .Build();

            await HttpContext.SignOutAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> ProfileAsync()
        {
            var response = _accountService.GetAccount(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value).Result;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var contentString = await response.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<CustomerAccount>(contentString, options);
            if(customer == null)
            {
                return View(new UserProfileViewModel());
            }
            var viewModel = new UserProfileViewModel
            {
                EmailAddress = User.Identity.Name,
                ProfileImage = User.Claims
                    .FirstOrDefault(c => c.Type == "picture")?.Value,
                Surname = customer.Surname,
                Forename = customer.Forename,
                Telephone = customer.Telephone,
                StreetAddress = customer.StreetAddress,
                City = customer.City,
                County = customer.County,
                PostalCode = customer.PostalCode,
                Balance = customer.Balance
            };
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task SignUpAsync(string returnUrl = "/Account/Profile")
        {
            var authenticationProperties = new
                LoginAuthenticationPropertiesBuilder()
                .WithParameter("screen_hint", "signup")
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileAsync(UserProfileViewModel model)
        {
            if (model.Surname != null || 
                model.Forename != null ||
                model.EmailAddress != null || 
                model.Telephone != null || 
                model.StreetAddress != null ||
                model.City != null || 
                model.County != null || 
                model.PostalCode != null)
            {
                var customer = new CustomerAccount
                {
                    AuthId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    Surname = model.Surname,
                    Forename = model.Forename,
                    Email = model.EmailAddress,
                    Telephone = model.Telephone,
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    County = model.County,
                    PostalCode = model.PostalCode,
                    Balance = 100.00
                };
                try
                {
                    var response = await _accountService.UpdateAccount(customer);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        response = await _accountService.CreateAccount(customer);
                    }

                    return RedirectToAction("Profile", model);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                } 
            }

            return View(model);
        }
    }
}
