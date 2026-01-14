namespace Basket.Domain.Entities;

public class ShoppingCart
{
    public string UserId { get; set; } = string.Empty;
    public List<ShoppingCartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ShoppingCart()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ShoppingCart(string userId)
    {
        UserId = userId;
        Items = new List<ShoppingCartItem>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class ShoppingCartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}