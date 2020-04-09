using EasyCiteLib.Repository.EasyCite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EasyCiteLib.Repository
{
    public class EasyCiteDbContext : DbContext
    {
        #region DbClasses
        #endregion

        private readonly IConfiguration _configuration;

        public EasyCiteDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var builder = new SqlConnectionStringBuilder(_configuration.GetConnectionString("EasyCite"));
            builder.Password = _configuration["EasyCitePassword"];
            builder.TrustServerCertificate = true;
            options.UseSqlServer(builder.ConnectionString, opts => opts.MigrationsAssembly("EasyCite"));
        }
    }
}