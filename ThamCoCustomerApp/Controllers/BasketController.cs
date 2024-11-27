using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.BasketService;

namespace ThamCoCustomerApp.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            HttpResponseMessage response = await _basketService.GetBasket(userId);
            if (!response.IsSuccessStatusCode)
            {
                return View(new BasketViewModel
                {
                    Products = new List<ProductViewModel>()
                });
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            string contentString = await response.Content.ReadAsStringAsync();
            BasketDto basket = null;

            try
            {
                basket = JsonSerializer.Deserialize<BasketDto>(contentString, options);
            }
            catch (JsonException ex)
            {
                // Log the exception (ex) as needed
                return View(new BasketViewModel
                {
                    Products = new List<ProductViewModel>()
                });
            }

            if (basket == null || basket.Products == null)
            {
                return View(new BasketViewModel
                {
                    Products = new List<ProductViewModel>()
                });
            }

            var viewModel = new BasketViewModel
            {
                Products = basket.Products.Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Brand = p.Brand,
                    Description = p.Description,
                    Price = p.Price
                }).ToList()
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddToBasket(ProductViewModel productViewModel)
        {
            var product = new CompanyWithProductDto
            {
                ProductId = productViewModel.ProductId,
                CompanyId = productViewModel.CompanyId,
                Name = productViewModel.Name,
                Brand = productViewModel.Brand,
                Description = productViewModel.Description,
                Price = productViewModel.Price

            };

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            HttpResponseMessage response = await _basketService.AddToBasket(userId, product);
            if (!response.IsSuccessStatusCode)
            {


                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(ProductViewModel productViewModel)
        {
            var product = new CompanyWithProductDto
            {
                ProductId = productViewModel.ProductId,
                CompanyId = productViewModel.CompanyId,
                Name = productViewModel.Name,
                Brand = productViewModel.Brand,
                Description = productViewModel.Description,
                Price = productViewModel.Price

            };

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            HttpResponseMessage response = await _basketService.RemoveFromBasket(userId, product);
            if (!response.IsSuccessStatusCode)
            {


                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ClearBasket()
        {


            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            HttpResponseMessage response = await _basketService.ClearBasket(userId);
            if (!response.IsSuccessStatusCode)
            {


                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
