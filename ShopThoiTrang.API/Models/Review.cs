namespace ShopThoiTrang.API.Models
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsHidden { get; set; } = false;
        public List<string>? ImageUrls { get; set; } = new List<string>();

        public List<string>? VideoUrls { get; set; } = new List<string>();

        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}
