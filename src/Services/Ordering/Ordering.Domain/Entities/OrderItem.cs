using Ordering.Domain.Common;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Entities;

public class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public Money UnitPrice { get; private set; } = new Money(0);
    public int Quantity { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;

    // Propiedad calculada
    public Money TotalPrice => UnitPrice * Quantity;

    // Relación con Order
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    private OrderItem() { } // Para EF Core

    public OrderItem(Guid productId, string productName, Money unitPrice, int quantity, string imageUrl)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        Quantity = quantity;
        ImageUrl = imageUrl ?? string.Empty;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
    }
}