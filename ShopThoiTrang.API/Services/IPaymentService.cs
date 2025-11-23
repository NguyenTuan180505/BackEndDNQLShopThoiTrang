using ShopThoiTrang.API.Dtos.Payment;

namespace ShopThoiTrang.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId);
        Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId, int userId);
        Task<PaymentDto?> GetByIdAsync(int id, int userId);
    }
}
