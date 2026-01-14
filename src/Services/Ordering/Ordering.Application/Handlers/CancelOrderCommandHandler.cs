using MediatR;
using Ordering.Application.Commands;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Handlers;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
            return false;

        order.CancelOrder();
        await _orderRepository.UpdateAsync(order);
        
        return true;
    }
}