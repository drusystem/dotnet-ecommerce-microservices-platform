using MediatR;
using Ordering.Application.Commands;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Handlers;

public class DeliverOrderCommandHandler : IRequestHandler<DeliverOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeliverOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
            return false;

        order.MarkAsDelivered();
        await _orderRepository.UpdateAsync(order);
        
        return true;
    }
}