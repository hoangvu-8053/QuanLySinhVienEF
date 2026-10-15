using QuanLySinhVienEF.DAL;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.BLL;

/// <summary>
/// BLL – Xử lý nghiệp vụ Sinh viên: validate đầu vào + điều phối DAL
/// </summary>
public class StudentService
{
    private readonly IStudentRepository _repo;

    // Constructor mặc định (dùng trong UI)
    public StudentService() : this(new StudentRepository()) { }

    // Constructor inject (dùng cho unit test)
    public StudentService(IStudentRepository repo) => _repo = repo;

    // ── READ ────────────────────────────────────────────────────────────
    public Task<List<Student>> GetAllAsync()    => _repo.GetAllAsync();
    public Task<List<Student>> GetPassedAsync() => _repo.GetPassedAsync();

    // ── CREATE ──────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Message)> AddAsync(string hoTen, string diemText)
    {
        var (ok, name, grade, err) = Validate(hoTen, diemText);
        if (!ok) return (false, err);

        await _repo.AddAsync(new Student { FullName = name, Grade = grade });
        return (true, $"✔ Đã thêm sinh viên \"{name}\".");
    }

    // ── UPDATE ──────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Message)> UpdateAsync(int id, string hoTen, string diemText)
    {
        var (ok, name, grade, err) = Validate(hoTen, diemText);
        if (!ok) return (false, err);

        await _repo.UpdateAsync(new Student { Id = id, FullName = name, Grade = grade });
        return (true, $"✔ Đã cập nhật sinh viên \"{name}\".");
    }

    // ── DELETE ──────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Message)> DeleteAsync(int id, string hoTen)
    {
        await _repo.DeleteAsync(id);
        return (true, $"✔ Đã xóa sinh viên \"{hoTen}\".");
    }

    // ── Validate dữ liệu đầu vào ────────────────────────────────────────
    private static (bool Ok, string Name, double Grade, string Error)
        Validate(string hoTen, string diemText)
    {
        string name = hoTen.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return (false, "", 0, "⚠ Họ tên không được để trống!");

        if (!double.TryParse(diemText.Trim(), out double grade) || grade < 0 || grade > 10)
            return (false, "", 0, "⚠ Điểm phải là số từ 0 đến 10!");

        return (true, name, grade, "");
    }
}
