using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;
using ThamCoCustomerApp.Services.OrderService;

namespace ThamCoCustomerApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            var authId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var orderResponse = await _orderService.GetOrders(authId);
            if (!orderResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var contentString = await orderResponse.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<IEnumerable<OrderDto>>(contentString, options);

            var orderViewModel = _mapper.Map<IEnumerable<OrderViewModel>>(orders);
            return View(orderViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var orderResponse = await _orderService.GetOrder(id);
            if (!orderResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var contentString = await orderResponse.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<OrderDto>(contentString, options);

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            return View(orderViewModel);
        }
    }
}
