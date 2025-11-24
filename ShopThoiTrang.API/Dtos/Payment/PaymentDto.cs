using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Payment
{
    public class PaymentDto
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionID { get; set; }
        public decimal Amount { get; set; }

        // Giữ nguyên local time như model
        public DateTime PaymentDate { get; set; }

        [Required]
        public string Status { get; set; } = default!;
    }
}
