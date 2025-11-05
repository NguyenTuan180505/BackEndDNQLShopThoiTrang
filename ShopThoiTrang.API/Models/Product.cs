namespace ShopThoiTrang.API.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public Category? Category { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
