using MediatR;
using Ordering.Application.Commands;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Handlers;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public ConfirmOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
            return false;

        order.ConfirmOrder();
        await _orderRepository.UpdateAsync(order);
        
        return true;
    }
}