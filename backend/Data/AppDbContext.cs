using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define os DbSets
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<ProductSupplier> ProductSuppliers { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!; // Adicionado para autenticação

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de chave composta para ProductSupplier (muitos-para-muitos)
            modelBuilder.Entity<ProductSupplier>()
                .HasKey(ps => new { ps.ProductsId, ps.SuppliersId });

            modelBuilder.Entity<ProductSupplier>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSuppliers)
                .HasForeignKey(ps => ps.ProductsId);

            modelBuilder.Entity<ProductSupplier>()
                .HasOne(ps => ps.Supplier)
                .WithMany(s => s.ProductSuppliers)
                .HasForeignKey(ps => ps.SuppliersId);

            // Configuração de relacionamento: Product -> Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dados de exemplo para Category
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Eletrônicos", Description = "Dispositivos eletrônicos" },
                new Category { Id = 2, Name = "Móveis", Description = "Móveis para casa e escritório" },
                new Category { Id = 3, Name = "Alimentos", Description = "Produtos alimentícios" }
            );

            // Dados de exemplo para Supplier
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Fornecedor A", Address = "Rua A", CNPJ = "12345678901234", Phone = "11987654321" },
                new Supplier { Id = 2, Name = "Fornecedor B", Address = "Rua B", CNPJ = "98765432109876", Phone = "21987654321" }
            );

            // Dados de exemplo para User (com senha hasheada)
            var passwordHasher = new PasswordHasher<User>();

            
        }
    }
}
