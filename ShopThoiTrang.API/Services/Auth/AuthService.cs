using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Services;
using ShopThoiTrang.API.Services.Auth;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Services.Impl;

namespace ShopThoiTrang.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;

        public AuthService(
            AppDbContext context,
            EmailService emailService,
            JwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        public async Task SendOtpAfterLoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Sai email hoặc mật khẩu");

            var otp = new Random().Next(100000, 999999).ToString();

            _context.EmailOtps.Add(new Models.EmailOtp
            {
                Email = email,
                OtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            await _context.SaveChangesAsync();
            await _emailService.SendOtpAsync(
            email,
            user.FullName ?? user.Email,
            otp
        );

        }

        public async Task<string> VerifyOtpAsync(string email, string otp)
        {
            var otpRecord = await _context.EmailOtps
                .Where(x => x.Email == email && !x.IsUsed && x.ExpiredAt > DateTime.UtcNow)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (otpRecord == null || !BCrypt.Net.BCrypt.Verify(otp, otpRecord.OtpHash))
                throw new Exception("OTP không hợp lệ hoặc hết hạn");

            otpRecord.IsUsed = true;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FirstAsync(x => x.Email == email);
            return _jwtService.GenerateToken(user);
        }
    }
}
