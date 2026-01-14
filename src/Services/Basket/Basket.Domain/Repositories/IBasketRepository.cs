using Basket.Domain.Entities;

namespace Basket.Domain.Repositories;

public interface IBasketRepository
{
    Task<ShoppingCart?> GetBasketAsync(string userId);
    Task<ShoppingCart> CreateOrUpdateBasketAsync(ShoppingCart basket);
    Task<bool> DeleteBasketAsync(string userId);
}