using MediatR;
using Ordering.Application.DTOs;

namespace Ordering.Application.Queries;

public record GetOrdersByUserQuery(string UserId) : IRequest<List<OrderDto>>;