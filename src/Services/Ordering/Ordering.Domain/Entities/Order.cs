using Ordering.Domain.Common;
using Ordering.Domain.Enums;
using Ordering.Domain.Events;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderItem> _orderItems = new();

    public string UserId { get; private set; } = string.Empty;
    public string UserEmail { get; private set; } = string.Empty;
    public Address ShippingAddress { get; private set; } = new Address("", "", "", "", "");
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    
    // Propiedad calculada
    public Money TotalAmount
    {
        get
        {
            if (!_orderItems.Any())
                return new Money(0);

            var total = _orderItems.First().UnitPrice.Amount * 0; // Inicializar con 0 de la misma moneda
            foreach (var item in _orderItems)
            {
                total += item.TotalPrice.Amount;
            }
            return new Money(total, _orderItems.First().UnitPrice.Currency);
        }
    }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { } // Para EF Core

    public Order(string userId, string userEmail, Address shippingAddress, List<OrderItem> orderItems)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("UserEmail cannot be empty", nameof(userEmail));

        if (orderItems == null || !orderItems.Any())
            throw new ArgumentException("Order must have at least one item", nameof(orderItems));

        UserId = userId;
        UserEmail = userEmail;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
        Status = OrderStatus.Pending;
        PaymentStatus = PaymentStatus.Pending;
        OrderDate = DateTime.UtcNow;

        _orderItems.AddRange(orderItems);

        // Evento de dominio
        AddDomainEvent(new OrderCreatedEvent(this));
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(this));
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        Status = OrderStatus.Shipped;
        ShippedDate = DateTime.UtcNow;
        AddDomainEvent(new OrderShippedEvent(this));
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
        AddDomainEvent(new OrderDeliveredEvent(this));
    }

    public void CancelOrder()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(this));
    }

    public void MarkPaymentAsPaid()
    {
        if (PaymentStatus == PaymentStatus.Paid)
            throw new InvalidOperationException("Payment is already marked as paid");

        PaymentStatus = PaymentStatus.Paid;
    }

    public void MarkPaymentAsFailed()
    {
        PaymentStatus = PaymentStatus.Failed;
    }
}