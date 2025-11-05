namespace ShopThoiTrang.API.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string OrderStatus { get; set; } = "Processing";

        public User? User { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
