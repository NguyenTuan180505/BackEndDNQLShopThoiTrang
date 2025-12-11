using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public string PaymentMethod { get; set; } = default!;

        // TransactionID có thể null (COD không có)
        public string? TransactionID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền không hợp lệ")]
        public decimal Amount { get; set; }
    }
}
