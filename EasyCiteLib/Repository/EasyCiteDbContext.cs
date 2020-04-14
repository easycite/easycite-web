using EasyCiteLib.Repository.EasyCite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EasyCiteLib.Repository
{
    public class EasyCiteDbContext : DbContext
    {
        #region Entities
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectReference> ProjectReferences { get; set; }
        public DbSet<ProjectHiddenResult> ProjectHiddenResults { get; set; }
        public DbSet<User> Users { get; set; }

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectReference>()
                .Property(pr => pr.IsPending)
                .HasDefaultValue(true);
            
            // Setup cascade deletes
            modelBuilder.Entity<ProjectReference>()
                .HasOne(pr => pr.Project)
                .WithMany(p => p.ProjectReferences)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectHiddenResult>()
                .HasOne(ph => ph.Project)
                .WithMany(p => p.ProjectHiddenResults)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}