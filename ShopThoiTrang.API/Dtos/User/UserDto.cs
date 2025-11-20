namespace ShopThoiTrang.API.Dtos.User
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
