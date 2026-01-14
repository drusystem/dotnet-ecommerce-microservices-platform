using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderCreatedEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}