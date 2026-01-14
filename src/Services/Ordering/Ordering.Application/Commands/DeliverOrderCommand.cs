using MediatR;

namespace Ordering.Application.Commands;

public record DeliverOrderCommand(Guid OrderId) : IRequest<bool>;