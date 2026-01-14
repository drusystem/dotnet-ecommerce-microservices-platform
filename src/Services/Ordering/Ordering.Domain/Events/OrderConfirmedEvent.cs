using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Events;

public class OrderConfirmedEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderConfirmedEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}