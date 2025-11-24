namespace ShopThoiTrang.API.Dtos.Order
{
    public class OrderItemResponseDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } 
        public string ProductImage { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 

        // Tính tổng phụ (Optional): Số lượng * Đơn giá
        public decimal SubTotal => Quantity * UnitPrice;
    }
}