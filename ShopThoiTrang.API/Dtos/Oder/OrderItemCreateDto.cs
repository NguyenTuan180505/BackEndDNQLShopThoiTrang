using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Order
{
    public class OrderItemCreateDto
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}