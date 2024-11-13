namespace ThamCoCustomerApp.Services
{
    public interface IProductService
    {
        Task<HttpResponseMessage> GetProduct(int productId);
        Task<HttpResponseMessage> GetProducts();
    }
}
