namespace ShopThoiTrang.API.Dtos.Review
{
    public class ReviewDto
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public string FullName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
