using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Events;

public class OrderDeliveredEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderDeliveredEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}