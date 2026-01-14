using MediatR;

namespace Ordering.Application.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<bool>;