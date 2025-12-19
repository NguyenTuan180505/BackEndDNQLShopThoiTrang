namespace ShopThoiTrang.API.Dtos.Oder
{
    public class CreateOrderFromSelectedCartDto
    {
        public List<int> CartItemIds { get; set; } = new();
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }

}
