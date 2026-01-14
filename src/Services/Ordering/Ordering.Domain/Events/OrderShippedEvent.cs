using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Events;

public class OrderShippedEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderShippedEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}