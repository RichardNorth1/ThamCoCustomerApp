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
using ThamCoCustomerApp.Dtos;
using static NuGet.Packaging.PackagingConstants;
using AutoMapper;

namespace ThamCoCustomerApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
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
            var customer = JsonSerializer.Deserialize<CustomerAccountDto>(contentString, options);
            if(customer == null)
            {
                return View(new UserProfileViewModel());
            }
            var customerViewModel = _mapper.Map<UserProfileViewModel>(customer);
            customerViewModel.EmailAddress = User.Identity.Name;
            customerViewModel.ProfileImage = User.Claims
                    .FirstOrDefault(c => c.Type == "picture")?.Value;
            return View(customerViewModel);
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
                model.Telephone != null || 
                model.StreetAddress != null ||
                model.City != null || 
                model.County != null || 
                model.PostalCode != null)
            {
                var customer = _mapper.Map<CustomerAccountDto>(model);
                customer.AuthId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

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
