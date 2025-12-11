using ShopThoiTrang.API.Dtos.Payment;

namespace ShopThoiTrang.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId);
        Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId, int userId, bool isAdmin);
        Task<PaymentDto?> GetByIdAsync(int id, int userId, bool isAdmin);
        Task<IEnumerable<PaymentDto>> GetAllAsync(); // Admin
    }

}
