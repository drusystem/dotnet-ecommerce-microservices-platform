using MediatR;

namespace Ordering.Application.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<bool>;