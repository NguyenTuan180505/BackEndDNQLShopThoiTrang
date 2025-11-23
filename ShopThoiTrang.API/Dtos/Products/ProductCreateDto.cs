namespace ShopThoiTrang.API.Dtos.Products
{
    public class ProductCreateDto
    {
        public int CategoryID { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } = 0;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }
}
