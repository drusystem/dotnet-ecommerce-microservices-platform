namespace Basket.Application.DTOs;

public record BasketDto(
    string UserId,
    List<BasketItemDto> Items,
    decimal TotalPrice,
    DateTime UpdatedAt
);

public record BasketItemDto(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    string ImageUrl
);

public record AddItemToBasketDto(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    string ImageUrl
);

public record UpdateItemQuantityDto(
    Guid ProductId,
    int Quantity
);