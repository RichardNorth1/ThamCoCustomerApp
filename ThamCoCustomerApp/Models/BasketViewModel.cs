namespace ThamCoCustomerApp.Models
{
    public class BasketViewModel
    {
        public double TotalPrice => Products?.Sum(p => p.Price) ?? 0;

        public List<ProductViewModel> Products { get; set; }
    }
}
