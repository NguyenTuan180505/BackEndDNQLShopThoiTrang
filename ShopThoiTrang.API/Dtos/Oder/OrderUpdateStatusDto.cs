using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class OrderUpdateStatusDto
    {
        [Required]
        public string OrderStatus { get; set; } // Ví dụ: "Shipped", "Delivered", "Cancelled"
    }
}