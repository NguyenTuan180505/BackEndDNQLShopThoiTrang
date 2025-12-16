namespace ShopThoiTrang.API.Services.Auth
{
    public interface IAuthService
    {
        Task SendOtpAfterLoginAsync(string email, string password);
        Task<string> VerifyOtpAsync(string email, string otp);
    }
}
