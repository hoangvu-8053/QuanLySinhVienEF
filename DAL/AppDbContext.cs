using Microsoft.EntityFrameworkCore;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.DAL;

/// <summary>
/// DAL – DbContext: cầu nối giữa EF Core và SQLite
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=QuanLySinhVien.db");
    }

    /// <summary>
    /// Tạo tất cả bảng nếu chưa có (an toàn với DB đã tồn tại)
    /// Dùng CREATE TABLE IF NOT EXISTS thay vì EnsureCreated/Migrate
    /// để không ảnh hưởng dữ liệu cũ.
    /// </summary>
    public async Task KhoiTaoBangAsync()
    {
        await Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS Students (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT    NOT NULL DEFAULT '',
                Grade    REAL    NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Products (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Name     TEXT    NOT NULL DEFAULT '',
                Price    DECIMAL NOT NULL DEFAULT 0,
                Quantity INTEGER NOT NULL DEFAULT 0
            );
        ");
    }
}
