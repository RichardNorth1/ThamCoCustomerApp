namespace ThamCoCustomerApp.Models
{
    public class ProductViewModel
    {

        public ProductViewModel()
        {

        }
        public ProductViewModel(int productId, int companyId, string name, string brand, string description, double price, string imageUrl)
        {
            ProductId = productId;
            CompanyId = companyId;
            Name = name;
            Brand = brand;
            Description = description;
            Price = price;
            ImageUrl = imageUrl;
        }

        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int StockLevel { get; set; }
    }
}
