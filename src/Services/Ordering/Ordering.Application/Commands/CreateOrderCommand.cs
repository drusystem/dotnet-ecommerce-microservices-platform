using MediatR;
using Ordering.Application.DTOs;

namespace Ordering.Application.Commands;

public record CreateOrderCommand(
    string UserId,
    string UserEmail,
    AddressDto ShippingAddress,
    List<CreateOrderItemDto> Items
) : IRequest<OrderDto>;