namespace ShopThoiTrang.API.Dtos.Products
{
    public class ProductUpdateDto
    {
        public int CategoryID { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
