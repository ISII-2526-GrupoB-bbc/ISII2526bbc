using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;
using AppForSEII2526.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace AppForSEII2526.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // ✅ Constructor clásico para runtime (DI)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ✅ Constructor sin parámetros para design-time (EF Tools)
        public ApplicationDbContext() { }

        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewItem> ReviewItems { get; set; }
        public DbSet<Model> Models { get; set; }

        // Alias para no romper controladores que usan _context.ApplicationUsers
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Si DI ya configuró el contexto (runtime), no toques nada
            if (optionsBuilder.IsConfigured) return;

            // Fallback para design-time (Add-Migration/Update-Database)
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var cfg = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var which = Environment.GetEnvironmentVariable("DBConnection2Use");

            if (string.Equals(which, "SQLite", StringComparison.OrdinalIgnoreCase))
            {
                // En migraciones usa archivo, no :memory:
                optionsBuilder.UseSqlite("Data Source=Application.db;Cache=Shared");
            }
            else if (string.Equals(which, "AzureSQL", StringComparison.OrdinalIgnoreCase))
            {
                var azure = Environment.GetEnvironmentVariable("AzureSQL");
                if (string.IsNullOrWhiteSpace(azure))
                    throw new InvalidOperationException("AzureSQL (env var) no configurada.");
                optionsBuilder.UseSqlServer(azure);
            }
            else
            {
                var conn = cfg.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");
                optionsBuilder.UseSqlServer(conn);
            }
        }
    }
}