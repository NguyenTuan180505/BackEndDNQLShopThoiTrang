using System.Data;

namespace ShopThoiTrang.API.Models
{
    public class User
    {
            public int UserID { get; set; }
            public string FullName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string? Phone { get; set; }
            public string PasswordHash { get; set; } = null!;
            public int RoleID { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public bool IsActive { get; set; } = true;

            public Role? Role { get; set; }
            public ICollection<Cart>? Carts { get; set; }
            public ICollection<Order>? Orders { get; set; }
            public ICollection<Review>? Reviews { get; set; }
        }
    }
