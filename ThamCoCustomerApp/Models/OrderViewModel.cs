using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public string OrderStatus { get; set; }
        public DateOnly OrderDate { get; set; }
        public double totalPrice => Products.Sum(p => p.Price);
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }
}
