using System.Net;
using System.Net.Mail;

namespace ShopThoiTrang.API.Services.Auth
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string toEmail, string fullName, string otp)
        {
            // Đọc cấu hình
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var portStr = _config["EmailSettings:Port"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];

            if (string.IsNullOrWhiteSpace(smtpServer))
                throw new Exception("Thiếu EmailSettings:SmtpServer");
            if (string.IsNullOrWhiteSpace(portStr) || !int.TryParse(portStr, out var port))
                throw new Exception("Thiếu hoặc sai EmailSettings:Port");
            if (string.IsNullOrWhiteSpace(senderEmail))
                throw new Exception("Thiếu EmailSettings:SenderEmail");
            if (string.IsNullOrWhiteSpace(senderPassword))
                throw new Exception("Thiếu EmailSettings:SenderPassword");
            var body = $@"
                Chào {fullName},

                Mã OTP của bạn là: {otp}

                Mã này sẽ hết hạn sau 5 phút.
                Vui lòng không chia sẻ mã này cho bất kỳ ai.

                Trân trọng,
                Team WEB: Web Ban Hang Thời Trang
                ";

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, "ShopThoiTrang"),
                Subject = "Mã OTP đăng nhập",
                Body = body
            };

            message.To.Add(toEmail);

            using var smtp = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
