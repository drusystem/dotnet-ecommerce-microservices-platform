using MediatR;
using Ordering.Application.DTOs;
using Ordering.Application.Queries;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
            return null;

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        var addressDto = new AddressDto(
            order.ShippingAddress.Street,
            order.ShippingAddress.City,
            order.ShippingAddress.State,
            order.ShippingAddress.Country,
            order.ShippingAddress.ZipCode
        );

        var itemDtos = order.OrderItems.Select(item => new OrderItemDto(
            item.Id,
            item.ProductId,
            item.ProductName,
            item.UnitPrice.Amount,
            item.Quantity,
            item.TotalPrice.Amount,
            item.ImageUrl
        )).ToList();

        return new OrderDto(
            order.Id,
            order.UserId,
            order.UserEmail,
            addressDto,
            itemDtos,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.Status,
            order.PaymentStatus,
            order.OrderDate,
            order.ShippedDate,
            order.DeliveredDate
        );
    }
}