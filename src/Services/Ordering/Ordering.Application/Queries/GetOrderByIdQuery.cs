using MediatR;
using Ordering.Application.DTOs;

namespace Ordering.Application.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;