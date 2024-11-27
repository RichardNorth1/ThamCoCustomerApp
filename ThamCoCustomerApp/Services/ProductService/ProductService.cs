using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net.Http.Headers;
using ThamCoCustomerApp.Services.Token;

namespace ThamCoCustomerApp.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        private readonly ITokenService _tokenService;

        public ProductService(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;

            _retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(6, onRetry: (response, retryCount) =>
                {
                });

            _circuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(5));
        }

        public async Task<HttpResponseMessage> GetProduct(int productId)
        {

            var response = await _retryPolicy.ExecuteAsync(() =>
                _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken().Result.AccessToken);
                    var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"/api/products/{productId}"));
                    return response;
                }));

            return response;
        }

        public async Task<HttpResponseMessage> GetProducts()
        {

            var response = await _retryPolicy.ExecuteAsync(() =>
                _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken().Result.AccessToken);
                    var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/products"));
                    return response;
                }));

            return response;
        }
    }
}

