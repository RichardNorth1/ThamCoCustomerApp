using System.Net;
using System.Text.Json;
using ThamCoCustomerApp.Dtos;

namespace ThamCoCustomerApp.Services.Product
{
    public class ProductServiceFake : IProductService
    {
        private readonly IEnumerable<CompanyWithProductDto> _products;
        public ProductServiceFake()
        {
            _products = new List<CompanyWithProductDto>
            {
                new CompanyWithProductDto
                {
                    ProductId = 1,
                    CompanyId = 1,
                    Name = "Product 1",
                    Brand = "Brand 1",
                    Description = "Description 1",
                    Price = 10,
                    ImageUrl = "imageUrl",
                    StockLevel = 10
                },
                new CompanyWithProductDto
                {
                    ProductId = 2,
                    CompanyId = 2,
                    Name = "Product 2",
                    Brand = "Brand 2",
                    Description = "Description 2",
                    Price = 20,
                    ImageUrl = "imageUrl",
                    StockLevel = 20
                },

                new CompanyWithProductDto
                {
                    ProductId = 3,
                    CompanyId = 1,
                    Name = "Product 3",
                    Brand = "Brand 3",
                    Description = "Description 3",
                    Price = 30,
                    ImageUrl = "imageUrl",
                    StockLevel = 30
                },
                new CompanyWithProductDto
                {
                    ProductId = 4,
                    CompanyId = 2,
                    Name = "Product 4",
                    Brand = "Brand 4",
                    Description = "Description 4",
                    Price = 40,
                    ImageUrl = "imageUrl",
                    StockLevel = 40
                },

                new CompanyWithProductDto
                {
                    ProductId = 5,
                    CompanyId = 1,
                    Name = "Product 5",
                    Brand = "Brand 5",
                    Description = "Description 5",
                    Price = 50,
                    ImageUrl = "imageUrl",
                    StockLevel = 50
                }
            };
        }

        public async Task<HttpResponseMessage> GetProduct(int productId)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_products.FirstOrDefault(p => p.ProductId == productId)))
            };
        }

        public async Task<HttpResponseMessage> GetProducts()
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_products))
            };
        }
    }
}
