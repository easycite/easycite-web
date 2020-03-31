using Autofac;
using EasyCiteLib;
using EasyCiteLib.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace EasyCite
{
    public class Startup
    {
        readonly IConfiguration _configuration;
        readonly IHostEnvironment _environment;
        readonly IWebHostEnvironment _webEnvironment;
        
        public Startup(IConfiguration configuration, IHostEnvironment environment, IWebHostEnvironment webEnvironment)
        {
            _configuration = configuration;
            _environment = environment;
            _webEnvironment = webEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews();

            builder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            if (_webEnvironment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }

            services.Configure<Neo4jOptions>(_configuration.GetSection("neo4j"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_webEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var containerConfiguration = new LibraryModule(_environment);
            builder.RegisterModule(containerConfiguration);
        }
    }
}