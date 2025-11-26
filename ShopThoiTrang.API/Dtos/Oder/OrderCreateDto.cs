using ShopThoiTrang.API.Dtos.Order;
using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class OrderCreateDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public string PaymentMethod { get; set; } = "COD";

        [Required]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
        public List<OrderItemInputDto> Items { get; set; } = new();

        public bool IsFromCart { get; set; } = false;
    }
}