using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Dtos.Payment;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

namespace ShopThoiTrang.API.Services.Impl
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentRepository _paymentRepo;

        public PaymentService(AppDbContext context, IPaymentRepository paymentRepo)
        {
            _context = context;
            _paymentRepo = paymentRepo;
        }

        // ===========================
        // 1. Create payment
        // ===========================
        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderID == dto.OrderID && o.UserID == userId);

            if (order == null)
                throw new Exception("Order không tồn tại hoặc không thuộc về bạn");

            // Giữ nguyên mặc định Status="Success" & PaymentDate = DateTime.Now
            var payment = new Payment
            {
                OrderID = dto.OrderID,
                PaymentMethod = dto.PaymentMethod,
                TransactionID = dto.TransactionID,
                Amount = dto.Amount
            };

            var created = await _paymentRepo.CreateAsync(payment);
            return Map(created);
        }


        // ===========================
        // 2. Lấy theo OrderID
        // ===========================
        public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId, int userId, bool isAdmin)
        {
            if (!isAdmin)
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderID == orderId && o.UserID == userId);

                if (order == null)
                    throw new Exception("Order không tồn tại hoặc không thuộc về bạn");
            }

            var list = await _paymentRepo.GetByOrderIdAsync(orderId);
            return list.Select(Map);
        }


        // ===========================
        // 3. Lấy theo PaymentID
        // ===========================
        public async Task<PaymentDto?> GetByIdAsync(int id, int userId, bool isAdmin)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return null;

            if (!isAdmin && payment.Order != null && payment.Order.UserID != userId)
                throw new Exception("Bạn không có quyền xem thanh toán này");

            return Map(payment);
        }


        // ===========================
        // 4. Admin: lấy tất cả
        // ===========================
        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var list = await _paymentRepo.GetAllAsync();
            return list.Select(Map);
        }


        // ===========================
        // Mapping DTOs
        // ===========================
        private PaymentDto Map(Payment p)
        {
            return new PaymentDto
            {
                PaymentID = p.PaymentID,
                OrderID = p.OrderID,
                PaymentMethod = p.PaymentMethod,
                TransactionID = p.TransactionID,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Status = p.Status
            };
        }
    }
}
