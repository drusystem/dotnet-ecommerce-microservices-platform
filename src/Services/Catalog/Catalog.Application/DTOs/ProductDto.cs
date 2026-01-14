namespace Catalog.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt
);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    Guid CategoryId
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    Guid CategoryId
);