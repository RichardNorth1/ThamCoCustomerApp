using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ThamCoCustomerApp.Services
{
    public class ProductService: IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;

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
                _circuitBreakerPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"/api/products/{productId}")));

            return response;
        }

        public async Task<HttpResponseMessage> GetProducts()
        {

            var response = await _retryPolicy.ExecuteAsync(() =>
                _circuitBreakerPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"/api/products")));

            return response;
        }
    }
}

