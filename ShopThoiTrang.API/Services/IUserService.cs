using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    public interface IUserService
    {
        Task<User?> GetByEmail(string email);
        Task<User?> GetById(int id);
        Task<List<User>> GetAll();
        Task<User> Register(User user, string password);
        bool CheckPassword(string hash, string password);
        Task UpdateRole(int id, int roleId);
        Task UpdateStatus(int id, bool isActive);
    }
}
