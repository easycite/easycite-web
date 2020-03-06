using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using EasyCiteLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EasyCite
{
    public class Startup
    {
        readonly IConfiguration _configuration;
        readonly IHostEnvironment _environment;
        
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Example}/{id?}");
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var containerConfiguration = new ContainerConfiguration(_environment, builder);

            var entryAssembly = Assembly.GetEntryAssembly();
            Debug.Assert(entryAssembly != null, "entryAssembly != null");
            
            IEnumerable<Assembly> assemblies = entryAssembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load);

            foreach (Assembly assembly in assemblies)
                containerConfiguration.RegisterAssembly(assembly);
        }
    }
}
