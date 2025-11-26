using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;
using ShopThoiTrang.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _repo.GetByEmail(email);
    }

    public async Task<User?> GetById(int id)
    {
        return await _repo.GetById(id);
    }

    public async Task<List<User>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<User> Register(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.RoleID = 2;
        await _repo.Add(user);
        return user;
    }

    // xin sửa hàm này để lấy tk admin trong db, xử lí cũ bỏ vào try catch
    public bool CheckPassword(string hash, string password)
    {
        // 1. Kiểm tra null
        if (string.IsNullOrEmpty(hash)) return false;

        // 2. Logic cho Admin Test (Pass thô: "admin123")
        if (!hash.StartsWith("$"))
        {
            return hash == password;
        }
        // 3. Logic cho User thật (Pass mã hóa BCrypt do Đăng ký qua API)
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    public async Task UpdateRole(int id, int roleId)
    {
        var user = await _repo.GetById(id);
        if (user == null) return;

        user.RoleID = roleId;
        await _repo.Update(user);
    }

    public async Task UpdateStatus(int id, bool isActive)
    {
        var user = await _repo.GetById(id);
        if (user == null) return;

        user.IsActive = isActive;
        await _repo.Update(user);
    }
}