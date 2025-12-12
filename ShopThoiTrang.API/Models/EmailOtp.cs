namespace ShopThoiTrang.API.Models
{
    public class EmailOtp
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string OtpHash { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
