using MediatR;
using Ordering.Application.Commands;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Handlers;

public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public ShipOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
            return false;

        order.MarkAsShipped();
        await _orderRepository.UpdateAsync(order);
        
        return true;
    }
}