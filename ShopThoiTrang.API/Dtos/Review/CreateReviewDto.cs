namespace ShopThoiTrang.API.Dtos.Review
{
    public class CreateReviewDto
    {
        public int ProductID { get; set; }
        public int Rating { get; set; }   // 1–5
        public string Comment { get; set; }
    }
}
