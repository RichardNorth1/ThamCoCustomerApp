
namespace ThamCoCustomerApp.Dtos
{
    public class BasketDto
    {
        public string CustomerId { get; set; }
        public List<CompanyWithProductDto> Products { get; set; } = new List<CompanyWithProductDto>();
    }
}
