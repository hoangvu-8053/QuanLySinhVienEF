using Microsoft.EntityFrameworkCore;

namespace QuanLySinhVienEF;

public class AppDbContext : DbContext
{
    public DbSet<Student> Students { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=QuanLySinhVien.db");
    }
}