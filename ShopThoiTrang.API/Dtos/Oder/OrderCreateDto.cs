using ShopThoiTrang.API.Dtos.Order;
using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public string PaymentMethod { get; set; }

        // Danh sách sản phẩm
        [Required]
        [MinLength(1, ErrorMessage = "Giỏ hàng phải có ít nhất 1 sản phẩm")]
        public List<OrderItemCreateDto> OrderItems { get; set; }
    }
}