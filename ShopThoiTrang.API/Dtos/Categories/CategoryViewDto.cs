namespace ShopThoiTrang.API.Dtos.Categories
{
    public class CategoryViewDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }

}
