using AutoMapper;
using ThamCoCustomerApp.Services;

namespace ThamCoCustomerApp
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            var autoMapperConfig = new MapperConfiguration(c => c.AddProfile(new MapperProfile()));
            IMapper mapper = autoMapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            if (_env.IsDevelopment())
            {
                services.AddSingleton<IProductService, ProductServiceFake>();
            }
            else
            {
                services.AddHttpClient<IProductService, ProductService>(client =>
                {
                    client.BaseAddress = new Uri(_configuration["WebServices:Products:BaseUrl"]);
                });
            }
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
