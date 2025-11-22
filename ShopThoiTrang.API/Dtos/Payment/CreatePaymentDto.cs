namespace ShopThoiTrang.API.Dtos.Payment
{
    public class CreatePaymentDto
    {
        public int OrderID { get; set; }
        public string PaymentMethod { get; set; }
        public string? TransactionID { get; set; }
        public decimal Amount { get; set; }
    }
}
