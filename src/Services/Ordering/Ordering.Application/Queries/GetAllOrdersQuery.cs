using MediatR;
using Ordering.Application.DTOs;

namespace Ordering.Application.Queries;

public record GetAllOrdersQuery : IRequest<List<OrderDto>>;