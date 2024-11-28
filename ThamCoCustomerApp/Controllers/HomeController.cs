using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.Product;

namespace ThamCoCustomerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public HomeController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            return View();
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var productResponse = await _productService.GetProduct(id);
            if (!productResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var contentString = await productResponse.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<CompanyWithProductDto>(contentString, options);

            var productViewModel = _mapper.Map<ProductViewModel>(product);
            return View(productViewModel);
        }

        // GET: Products/GetProducts
        public async Task<IActionResult> GetProducts(string searchString)
        {
            var productResponse = await _productService.GetProducts();
            if (!productResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)productResponse.StatusCode, productResponse.ReasonPhrase);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var contentString = await productResponse.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<CompanyWithProductDto>>(contentString, options);

            var productViewModels = _mapper.Map<List<ProductViewModel>>(products);

            if (!string.IsNullOrEmpty(searchString))
            {
                productViewModels = productViewModels
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                p.Brand.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Json(productViewModels);
        }
    }
}
