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

    public bool CheckPassword(string hash, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
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