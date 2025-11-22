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

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderID == dto.OrderID && o.UserID == userId);

            if (order == null)
                throw new Exception("Order không tồn tại hoặc không thuộc về bạn");

            var payment = new Payment
            {
                OrderID = dto.OrderID,
                PaymentMethod = dto.PaymentMethod,
                TransactionID = dto.TransactionID,
                Amount = dto.Amount,
                Status = "Success"
            };

            var created = await _paymentRepo.CreateAsync(payment);

            return Map(created);
        }

        public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderID == orderId && o.UserID == userId);

            if (order == null)
                throw new Exception("Order không tồn tại hoặc không thuộc về bạn");

            var list = await _paymentRepo.GetByOrderIdAsync(orderId);
            return list.Select(Map);
        }

        public async Task<PaymentDto?> GetByIdAsync(int id, int userId)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return null;

            if (payment.Order.UserID != userId)
                throw new Exception("Bạn không có quyền xem thanh toán này");

            return Map(payment);
        }

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
