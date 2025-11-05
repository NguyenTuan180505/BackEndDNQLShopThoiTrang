namespace ShopThoiTrang.API.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
