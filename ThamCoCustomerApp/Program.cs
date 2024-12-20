using Auth0.AspNetCore.Authentication;
using AutoMapper;
using Polly.Extensions.Http;
using Polly;
using ThamCoCustomerApp;
using ThamCoCustomerApp.Services.Account;
using ThamCoCustomerApp.Services.BasketService;
using ThamCoCustomerApp.Services.OrderService;
using ThamCoCustomerApp.Services.Product;
using ThamCoCustomerApp.Services.Token;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure AutoMapper
var autoMapperConfig = new MapperConfiguration(c => c.AddProfile(new MapperProfile()));
IMapper mapper = autoMapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth:CustomerWebApp:Domain"];
    options.ClientId = builder.Configuration["Auth:CustomerWebApp:ClientId"];
});
builder.Services.AddAuthorization();


// Register IProductService based on the environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IOrderService, OrderServiceFake>();
    builder.Services.AddScoped<IProductService, ProductServiceFake>();
    builder.Services.AddSingleton<IAccountService, AccountServiceFake>();
    builder.Services.AddSingleton<IBasketService, BasketServiceFake>();
}
else
{
    builder.Services.AddScoped<IOrderService, OrderServiceFake>();
    builder.Services.AddScoped<IAccountService, AccountServiceFake>();
    builder.Services.AddSingleton<IBasketService, BasketServiceFake>();
    builder.Services.AddHttpClient<IProductService, ProductService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["WebServices:CheapestProducts:BaseUrl"]);
    }).AddPolicyHandler(GetRetryPolicy())
                    .AddPolicyHandler(GetCircuitBreakerPolicy());

    builder.Services.AddSingleton<ITokenService, CheapestProductTokenService>();

}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}