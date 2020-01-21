using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VaderHinna.AzureService;
using VaderHinna.Helpers;
using VaderHinna.Model.Interface;

namespace VaderHinna
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionString"];
            var rootDir = Configuration["RootDirectory"];
            var discoveryFile = Configuration["DiscoveryFile"];

            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter()));
            services.AddApiVersioning();
            services.AddTransient<ICsvService, CsvService>();
            services.AddTransient<IAzureConnector>(s =>
                new AzureConnector(connectionString, rootDir, discoveryFile, s.GetService<ICsvService>()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
