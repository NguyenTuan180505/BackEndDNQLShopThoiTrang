using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class CreateOrderFromCartDto
    {
        [Required]
        public string ShippingAddress { get; set; } = null!;
        [Required]
        public string PaymentMethod { get; set; } = "COD"; // Mặc định COD
        public string? Note { get; set; }
    }
}