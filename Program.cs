using QuanLySinhVienEF.UI;

namespace QuanLySinhVienEF;

/// <summary>
/// Điểm khởi động ứng dụng — chỉ chứa hàm Main()
/// </summary>
static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormMenu());
    }
}
