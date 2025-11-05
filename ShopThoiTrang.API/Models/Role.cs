namespace ShopThoiTrang.API.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }

        // 🔁 Quan hệ 1-n với User
        public ICollection<User>? Users { get; set; }
    }
}
