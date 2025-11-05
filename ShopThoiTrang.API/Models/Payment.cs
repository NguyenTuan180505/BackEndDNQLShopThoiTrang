namespace ShopThoiTrang.API.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Success";

        public Order? Order { get; set; }
    }
}
