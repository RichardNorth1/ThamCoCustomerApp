namespace ThamCoCustomerApp.Services.Product
{
    public interface IProductService
    {
        Task<HttpResponseMessage> GetProduct(int productId);
        Task<HttpResponseMessage> GetProducts();
    }
}
