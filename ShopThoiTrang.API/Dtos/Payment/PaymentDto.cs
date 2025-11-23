namespace ShopThoiTrang.API.Dtos.Payment
{
    public class PaymentDto
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
    }
}
