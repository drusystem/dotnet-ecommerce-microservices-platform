using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
{
    // IDs fijos para que EF pueda rastrear los cambios
    var categoryId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    var categoryId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    
    var productId1 = Guid.Parse("33333333-3333-3333-3333-333333333333");
    var productId2 = Guid.Parse("44444444-4444-4444-4444-444444444444");
    
    // Fecha fija para el seed
    var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    modelBuilder.Entity<Category>().HasData(
        new Category
        {
            Id = categoryId1,
            Name = "Electrónicos",
            Description = "Dispositivos electrónicos y accesorios",
            CreatedAt = seedDate
        },
        new Category
        {
            Id = categoryId2,
            Name = "Ropa",
            Description = "Prendas de vestir",
            CreatedAt = seedDate
        }
    );

    modelBuilder.Entity<Product>().HasData(
        new Product
        {
            Id = productId1,
            Name = "Laptop Dell XPS 13",
            Description = "Laptop ultradelgada con procesador Intel i7",
            Price = 1299.99m,
            Stock = 15,
            ImageUrl = "https://via.placeholder.com/300x300?text=Dell+XPS+13",
            CategoryId = categoryId1,
            CreatedAt = seedDate,
            IsActive = true
        },
        new Product
        {
            Id = productId2,
            Name = "iPhone 15 Pro",
            Description = "Smartphone Apple con chip A17 Pro",
            Price = 999.99m,
            Stock = 25,
            ImageUrl = "https://via.placeholder.com/300x300?text=iPhone+15",
            CategoryId = categoryId1,
            CreatedAt = seedDate,
            IsActive = true
        }
    );
}
}