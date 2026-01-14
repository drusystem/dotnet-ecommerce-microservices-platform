using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.UserEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        // Configurar Value Object Address como Owned Entity
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ShippingStreet").IsRequired().HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("ShippingCity").IsRequired().HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("ShippingState").IsRequired().HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("ShippingCountry").IsRequired().HasMaxLength(100);
            address.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").IsRequired().HasMaxLength(20);
        });

        // Relación con OrderItems (uno a muchos)
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorar la propiedad calculada
        builder.Ignore(o => o.TotalAmount);

        // Ignorar eventos de dominio (no se persisten)
        builder.Ignore(o => o.DomainEvents);
    }
}