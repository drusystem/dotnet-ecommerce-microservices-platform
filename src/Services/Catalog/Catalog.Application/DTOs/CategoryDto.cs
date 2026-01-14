namespace Catalog.Application.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt
);

public record CreateCategoryDto(
    string Name,
    string Description
);