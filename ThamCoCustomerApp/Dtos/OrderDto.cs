namespace ThamCoCustomerApp.Dtos
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public string OrderStatus { get; set; }
        public DateOnly OrderDate { get; set; }
        public IEnumerable<CompanyWithProductDto> Products { get; set; } = new List<CompanyWithProductDto>();
    }
}
