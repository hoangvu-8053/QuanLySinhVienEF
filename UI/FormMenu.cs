using System.Drawing;
using System.Windows.Forms;

namespace QuanLySinhVienEF.UI;

/// <summary>
/// UI – Màn hình chọn module (điểm vào ứng dụng)
/// </summary>
public class FormMenu : Form
{
    public FormMenu()
    {
        Text            = "Ứng dụng Quản lý — EF Core Code First";
        ClientSize      = new Size(420, 270);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.FromArgb(245, 247, 250);
        Font            = new Font("Segoe UI", 10f);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;

        Controls.Add(new Label
        {
            Text      = "Chọn module muốn mở:",
            Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 60, 114),
            Location  = new Point(20, 28),
            AutoSize  = true
        });

        var btnSinhVien = TaoNut("👨‍🎓  Quản lý Sinh viên",    Color.FromArgb(0,  112, 200), 88);
        var btnSanPham  = TaoNut("📦  Quản lý Sản phẩm Kho", Color.FromArgb(34, 139,  34), 150);

        btnSinhVien.Click += (_, _) => new FormStudent().Show();
        btnSanPham.Click  += (_, _) => new FormProduct().Show();

        Controls.Add(btnSinhVien);
        Controls.Add(btnSanPham);

        Controls.Add(new Label
        {
            Text      = "EF Core 9  ·  SQLite  ·  Code First  ·  Mô hình 3 lớp",
            ForeColor = Color.Gray,
            Location  = new Point(20, 225),
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic)
        });
    }

    private static Button TaoNut(string text, Color bg, int y)
    {
        var btn = new Button
        {
            Text      = text,
            Location  = new Point(20, y),
            Size      = new Size(375, 48),
            BackColor = bg,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(12, 0, 0, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }
}
