using Ordering.Domain.Enums;

namespace Ordering.Application.DTOs;

public record OrderDto(
    Guid Id,
    string UserId,
    string UserEmail,
    AddressDto ShippingAddress,
    List<OrderItemDto> Items,
    decimal TotalAmount,
    string Currency,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    DateTime OrderDate,
    DateTime? ShippedDate,
    DateTime? DeliveredDate
);

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    string ImageUrl
);

public record AddressDto(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);

public record CreateOrderDto(
    string UserId,
    string UserEmail,
    AddressDto ShippingAddress,
    List<CreateOrderItemDto> Items
);

public record CreateOrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    string ImageUrl
);

public record UpdateOrderStatusDto(
    OrderStatus Status
);