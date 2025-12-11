using ShopThoiTrang.API.Dtos.Order;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class OrderResponseDto
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }

        // Danh sách chi tiết sản phẩm đi kèm
        public List<OrderItemResponseDto> OrderItems { get; set; }
    }
}