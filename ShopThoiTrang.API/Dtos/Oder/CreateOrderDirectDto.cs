using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class CreateOrderDirectDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;
        [Required]
        public string PaymentMethod { get; set; } = "COD";

        // Danh sách sản phẩm mua trực tiếp
        public List<OrderItemInputDto> Items { get; set; } = new();
    }

    public class OrderItemInputDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}