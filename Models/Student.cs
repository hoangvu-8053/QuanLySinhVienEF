namespace QuanLySinhVienEF.Models;

/// <summary>
/// Entity: Sinh viên
/// </summary>
public class Student
{
    public int    Id       { get; set; }
    public string FullName { get; set; } = string.Empty;
    public double Grade    { get; set; }
}
