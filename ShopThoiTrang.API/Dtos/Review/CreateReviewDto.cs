using System.ComponentModel.DataAnnotations;

namespace ShopThoiTrang.API.Dtos.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int ProductID { get; set; }

        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment tối đa 500 ký tự.")]
        public string? Comment { get; set; }

        public List<string>? ImageUrls { get; set; }

        public List<string>? VideoUrls { get; set; }
    }
}
