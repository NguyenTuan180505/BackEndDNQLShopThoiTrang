using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== FIX DECIMAL WARNINGS ====================
            // Tất cả các thuộc tính decimal cần chỉ định precision/scale
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);

                entity.Property(p => p.Discount)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.Property(ci => ci.UnitPrice)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.TotalAmount)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Amount)
                      .HasPrecision(18, 2);
            });

            // ==================== CẤU HÌNH REVIEW (nếu cần) ====================
            // Nếu bạn muốn set default cho IsHidden hoặc cấu hình kiểu dữ liệu cho ImageUrls/VideoUrls
            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(r => r.IsHidden)
                      .HasDefaultValue(false);  // Mặc định không ẩn

                // Nếu ImageUrls và VideoUrls là string (nvarchar(max))
                entity.Property(r => r.ImageUrls)
                      .HasColumnType("nvarchar(max)");

                entity.Property(r => r.VideoUrls)
                      .HasColumnType("nvarchar(max)");

                // Hoặc nếu bạn lưu dưới dạng JSON (khuyên dùng nếu là danh sách URL)
                // entity.Property(r => r.ImageUrls).HasColumnType("nvarchar(max)");
                // entity.Property(r => r.VideoUrls).HasColumnType("nvarchar(max)");
            });

            // Có thể thêm các cấu hình khác cho các entity khác nếu cần
        }
    }
}