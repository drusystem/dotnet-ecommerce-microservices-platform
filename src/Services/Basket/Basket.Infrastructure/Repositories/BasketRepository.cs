using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Basket.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<BasketRepository> _logger;

    public BasketRepository(IConnectionMultiplexer redis, ILogger<BasketRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<ShoppingCart?> GetBasketAsync(string userId)
    {
        try
        {
            var data = await _database.StringGetAsync(userId);
            
            if (data.IsNullOrEmpty)
                return null;

            var basket = JsonConvert.DeserializeObject<ShoppingCart>(data!);
            return basket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el carrito para el usuario {UserId}", userId);
            throw;
        }
    }

    public async Task<ShoppingCart> CreateOrUpdateBasketAsync(ShoppingCart basket)
    {
        try
        {
            basket.UpdatedAt = DateTime.UtcNow;
            
            var json = JsonConvert.SerializeObject(basket);
            var created = await _database.StringSetAsync(basket.UserId, json);

            if (!created)
            {
                _logger.LogError("Error al crear/actualizar el carrito para el usuario {UserId}", basket.UserId);
                throw new Exception("No se pudo guardar el carrito");
            }

            return basket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el carrito para el usuario {UserId}", basket.UserId);
            throw;
        }
    }

    public async Task<bool> DeleteBasketAsync(string userId)
    {
        try
        {
            return await _database.KeyDeleteAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el carrito para el usuario {UserId}", userId);
            throw;
        }
    }
}