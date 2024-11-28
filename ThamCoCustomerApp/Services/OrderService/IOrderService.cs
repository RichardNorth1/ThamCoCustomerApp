namespace ThamCoCustomerApp.Services.OrderService
{
    public interface IOrderService
    {
        public Task<HttpResponseMessage> GetOrder(int orderId);
        public Task<HttpResponseMessage> GetOrders(string customerId);
    }
}
