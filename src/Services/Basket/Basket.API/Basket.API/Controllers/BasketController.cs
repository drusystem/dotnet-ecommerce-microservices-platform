using Basket.Application.DTOs;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<BasketDto>> GetBasket(string userId)
    {
        var basket = await _basketRepository.GetBasketAsync(userId);
        
        if (basket == null)
            return Ok(new BasketDto(userId, new List<BasketItemDto>(), 0, DateTime.UtcNow));

        var basketDto = MapToDto(basket);
        return Ok(basketDto);
    }

    [HttpPost("{userId}/items")]
    public async Task<ActionResult<BasketDto>> AddItemToBasket(
        string userId, 
        [FromBody] AddItemToBasketDto itemDto)
    {
        var basket = await _basketRepository.GetBasketAsync(userId) 
                     ?? new ShoppingCart(userId);

        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += itemDto.Quantity;
        }
        else
        {
            basket.Items.Add(new ShoppingCartItem
            {
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                Price = itemDto.Price,
                Quantity = itemDto.Quantity,
                ImageUrl = itemDto.ImageUrl
            });
        }

        var updatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket);
        var basketDto = MapToDto(updatedBasket);
        
        return Ok(basketDto);
    }

    [HttpPut("{userId}/items")]
    public async Task<ActionResult<BasketDto>> UpdateItemQuantity(
        string userId,
        [FromBody] UpdateItemQuantityDto updateDto)
    {
        var basket = await _basketRepository.GetBasketAsync(userId);
        
        if (basket == null)
            return NotFound(new { message = "Carrito no encontrado" });

        var item = basket.Items.FirstOrDefault(i => i.ProductId == updateDto.ProductId);
        
        if (item == null)
            return NotFound(new { message = "Producto no encontrado en el carrito" });

        item.Quantity = updateDto.Quantity;
        
        var updatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket);
        var basketDto = MapToDto(updatedBasket);
        
        return Ok(basketDto);
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<ActionResult<BasketDto>> RemoveItemFromBasket(string userId, Guid productId)
    {
        var basket = await _basketRepository.GetBasketAsync(userId);
        
        if (basket == null)
            return NotFound(new { message = "Carrito no encontrado" });

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
            return NotFound(new { message = "Producto no encontrado en el carrito" });

        basket.Items.Remove(item);
        
        var updatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket);
        var basketDto = MapToDto(updatedBasket);
        
        return Ok(basketDto);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteBasket(string userId)
    {
        var result = await _basketRepository.DeleteBasketAsync(userId);
        
        if (!result)
            return NotFound(new { message = "Carrito no encontrado" });

        return NoContent();
    }

    private static BasketDto MapToDto(ShoppingCart basket)
    {
        var items = basket.Items.Select(i => new BasketItemDto(
            i.ProductId,
            i.ProductName,
            i.Price,
            i.Quantity,
            i.ImageUrl
        )).ToList();

        return new BasketDto(
            basket.UserId,
            items,
            basket.TotalPrice,
            basket.UpdatedAt
        );
    }
}