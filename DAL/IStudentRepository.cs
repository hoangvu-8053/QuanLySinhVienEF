using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.DAL;

/// <summary>
/// DAL – Interface định nghĩa các thao tác dữ liệu cho Sinh viên
/// </summary>
public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?>      GetByIdAsync(int id);
    Task                AddAsync(Student student);
    Task                UpdateAsync(Student student);
    Task                DeleteAsync(int id);
    Task<List<Student>> GetPassedAsync();   // LINQ: điểm >= 5
}
