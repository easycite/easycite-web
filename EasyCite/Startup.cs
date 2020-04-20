using System.Linq;
using System.Security.Claims;
using Autofac;
using EasyCiteLib;
using EasyCiteLib.Configuration;
using EasyCiteLib.Interface.Account;
using EasyCiteLib.Models.Account;
using EasyCiteLib.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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

            // Database configuration
            services.Configure<Neo4jOptions>(_configuration.GetSection("neo4j"));
            services.AddDbContext<EasyCiteDbContext>();
            services.AddTransient(typeof(IGenericDataContextAsync<>), typeof(GenericDataContextAsync<>));

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.AddHttpClient();
            services.AddMemoryCache();

            // Google login configuration
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    var googleAuthNSection = _configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.Events.OnCreatingTicket = async ctx =>
                    {
                        var userData = new UserSaveData
                        {
                            ProviderKey = ctx.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                            Email = ctx.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                            Firstname = ctx.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                            Lastname = ctx.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value
                        };

                        var createUserProcessor = ctx.HttpContext.RequestServices.GetRequiredService<ICreateUserProcessor>();

                        var userId = await createUserProcessor.CreateIfNotExistsAsync(userData);

                        ctx.Identity.AddClaim(new Claim(ClaimTypes.Sid, userId.ToString()));
                    };
                });
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
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var containerConfiguration = new LibraryModule(_environment);
            builder.RegisterModule(containerConfiguration);
        }
    }
}