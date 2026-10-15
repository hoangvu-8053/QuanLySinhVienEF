using Microsoft.EntityFrameworkCore;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.DAL;

/// <summary>
/// DAL – Triển khai thao tác CSDL cho Sinh viên (async/await + EF Core)
/// </summary>
public class StudentRepository : IStudentRepository
{
    // ── READ ────────────────────────────────────────────────────────────
    public async Task<List<Student>> GetAllAsync()
    {
        using var ctx = new AppDbContext();
        await ctx.KhoiTaoBangAsync();
        return await ctx.Students.ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var ctx = new AppDbContext();
        return await ctx.Students.FindAsync(id);
    }

    // ── CREATE ──────────────────────────────────────────────────────────
    public async Task AddAsync(Student student)
    {
        using var ctx = new AppDbContext();
        ctx.Students.Add(student);
        await ctx.SaveChangesAsync();
    }

    // ── UPDATE ──────────────────────────────────────────────────────────
    public async Task UpdateAsync(Student student)
    {
        using var ctx = new AppDbContext();
        Student? existing = await ctx.Students.FindAsync(student.Id);
        if (existing is null) return;
        existing.FullName = student.FullName;
        existing.Grade    = student.Grade;
        await ctx.SaveChangesAsync();
    }

    // ── DELETE ──────────────────────────────────────────────────────────
    public async Task DeleteAsync(int id)
    {
        using var ctx = new AppDbContext();
        Student? student = await ctx.Students.FindAsync(id);
        if (student is null) return;
        ctx.Students.Remove(student);
        await ctx.SaveChangesAsync();
    }

    // ── LINQ to Entities: Lọc sinh viên đạt ────────────────────────────
    public async Task<List<Student>> GetPassedAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Students
                        .Where(s => s.Grade >= 5)
                        .OrderByDescending(s => s.Grade)
                        .ToListAsync();
    }
}
