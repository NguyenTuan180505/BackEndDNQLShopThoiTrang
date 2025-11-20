using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Services;

namespace ShopThoiTrang.API.Repositories.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _db.Users.Include(r => r.Role)
                                  .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetById(int id)
        {
            return await _db.Users.Include(r => r.Role)
                                  .FirstOrDefaultAsync(x => x.UserID == id);
        }

        public async Task<List<User>> GetAll()
        {
            return await _db.Users.Include(r => r.Role).ToListAsync();
        }

        public async Task Add(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
