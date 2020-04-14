using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace EasyCiteLib.Repository
{
    public class EasyCiteDbContext : DbContext
    {
        #region Entities
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectReference> ProjectReferences { get; set; }
        public DbSet<Reference> References { get; set; }
        public DbSet<ReferenceSource> ReferenceSources { get; set; }
        public DbSet<Source> Sources { get; set; }
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
            // Seed Source table with values from SourceEnum
            modelBuilder.Entity<Source>().HasData(
                Enum.GetValues(typeof(SourceEnum))
                .OfType<SourceEnum>()
                .Select(se => new Source
                {
                    Id = se,
                    Name = se.ToString()
                })
            );

            // Setup cascade deletes
            modelBuilder.Entity<ProjectReference>()
                .HasOne(pr => pr.Project)
                .WithMany(p => p.ProjectReferences)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}