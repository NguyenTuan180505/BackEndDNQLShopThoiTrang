namespace ShopThoiTrang.API.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public User? User { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
