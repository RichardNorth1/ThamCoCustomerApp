using System.Net.Http.Headers;
using ThamCoCustomerApp.Services.Token;

namespace ThamCoCustomerApp.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public ProductService(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        public async Task<HttpResponseMessage> GetProduct(int productId)
        {

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken().Result.AccessToken);

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"/api/products/{productId}"));

            return response;
        }

        public async Task<HttpResponseMessage> GetProducts()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken().Result.AccessToken);

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/products"));

            return response;
        }
    }
}

