using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Events;

public class OrderCancelledEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderCancelledEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}