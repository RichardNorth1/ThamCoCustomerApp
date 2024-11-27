using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Services.BasketService
{
    public interface IBasketService
    {
        public Task<HttpResponseMessage> GetBasket(string customerId);
        public Task<HttpResponseMessage> AddToBasket(string customerId, CompanyWithProductDto product);
        public Task<HttpResponseMessage> RemoveFromBasket(string customerId,CompanyWithProductDto product);
        public Task<HttpResponseMessage> ClearBasket(string customerId);

    }
}
