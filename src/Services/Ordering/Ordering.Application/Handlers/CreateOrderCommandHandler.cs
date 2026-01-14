using MediatR;
using Ordering.Application.Commands;
using Ordering.Application.DTOs;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Crear el Address (Value Object)
        var shippingAddress = new Address(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.Country,
            request.ShippingAddress.ZipCode
        );

        // Crear los OrderItems
        var orderItems = request.Items.Select(item => new OrderItem(
            item.ProductId,
            item.ProductName,
            new Money(item.UnitPrice),
            item.Quantity,
            item.ImageUrl
        )).ToList();

        // Crear el Order (Aggregate Root)
        var order = new Order(
            request.UserId,
            request.UserEmail,
            shippingAddress,
            orderItems
        );

        // Guardar en el repositorio
        var createdOrder = await _orderRepository.CreateAsync(order);

        // Mapear a DTO
        return MapToDto(createdOrder);
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