using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Windows.Forms;

namespace QuanLySinhVienEF;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}

public class Form1 : Form
{
    private readonly Label lblTieuDe = new Label();
    private readonly TextBox txtHoTen = new TextBox();
    private readonly TextBox txtDiem = new TextBox();
    private readonly Button btnThem = new Button();
    private readonly Button btnSua = new Button();
    private readonly Button btnXoa = new Button();
    private readonly Button btnTaiLai = new Button();
    private readonly Button btnLocDat = new Button();
    private readonly DataGridView dgvSinhVien = new DataGridView();
    private readonly Label lblTrangThai = new Label();

    public Form1()
    {
        Text = "Quản lý Sinh viên - EF Core (SQLite) CRUD";
        ClientSize = new Size(700, 560);
        StartPosition = FormStartPosition.CenterScreen;

        XayDungGiaoDien();

        Load += Form1_Load;
        dgvSinhVien.SelectionChanged += DgvSinhVien_SelectionChanged;
    }

    private void XayDungGiaoDien()
    {
        lblTieuDe.Text = "QUẢN LÝ SINH VIÊN - EF CORE (CRUD ĐẦY ĐỦ)";
        lblTieuDe.Font = new Font(Font, FontStyle.Bold);
        lblTieuDe.Location = new Point(15, 15);
        lblTieuDe.AutoSize = true;
        Controls.Add(lblTieuDe);

        Controls.Add(new Label { Text = "Họ tên:", Location = new Point(15, 55), AutoSize = true });
        txtHoTen.Location = new Point(90, 52); txtHoTen.Size = new Size(260, 25);
        Controls.Add(txtHoTen);

        Controls.Add(new Label { Text = "Điểm:", Location = new Point(370, 55), AutoSize = true });
        txtDiem.Location = new Point(420, 52); txtDiem.Size = new Size(100, 25);
        Controls.Add(txtDiem);

        btnThem.Text = "+ Thêm"; btnThem.Location = new Point(15, 90); btnThem.Size = new Size(100, 32);
        btnThem.BackColor = Color.FromArgb(46, 160, 67); btnThem.ForeColor = Color.White; btnThem.FlatStyle = FlatStyle.Flat;
        btnThem.Click += BtnThem_Click;

        btnSua.Text = "Sửa"; btnSua.Location = new Point(125, 90); btnSua.Size = new Size(100, 32);
        btnSua.BackColor = Color.FromArgb(0, 120, 215); btnSua.ForeColor = Color.White; btnSua.FlatStyle = FlatStyle.Flat;
        btnSua.Click += BtnSua_Click;

        btnXoa.Text = "Xóa"; btnXoa.Location = new Point(235, 90); btnXoa.Size = new Size(100, 32);
        btnXoa.BackColor = Color.FromArgb(200, 40, 40); btnXoa.ForeColor = Color.White; btnXoa.FlatStyle = FlatStyle.Flat;
        btnXoa.Click += BtnXoa_Click;

        btnTaiLai.Text = "Tải lại danh sách"; btnTaiLai.Location = new Point(345, 90); btnTaiLai.Size = new Size(140, 32);
        btnTaiLai.Click += async (s, e) => await TaiDanhSach();

        btnLocDat.Text = "Lọc SV đạt (LINQ)"; btnLocDat.Location = new Point(495, 90); btnLocDat.Size = new Size(140, 32);
        btnLocDat.Click += BtnLocDat_Click;

        Controls.Add(btnThem); Controls.Add(btnSua); Controls.Add(btnXoa);
        Controls.Add(btnTaiLai); Controls.Add(btnLocDat);

        dgvSinhVien.Location = new Point(15, 135);
        dgvSinhVien.Size = new Size(660, 340);
        dgvSinhVien.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvSinhVien.ReadOnly = true;
        dgvSinhVien.AllowUserToAddRows = false;
        dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSinhVien.MultiSelect = false;
        dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Controls.Add(dgvSinhVien);

        lblTrangThai.Location = new Point(15, 485);
        lblTrangThai.AutoSize = true;
        lblTrangThai.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        Controls.Add(lblTrangThai);
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        lblTrangThai.Text = "Sẵn sàng. Dữ liệu lưu trong file QuanLySinhVien.db (SQLite).";
        await TaiDanhSach();
    }

    // ===== READ =====
    private async Task TaiDanhSach()
    {
        try
        {
            using var context = new AppDbContext();
            List<Student> danhSach = await context.Students.ToListAsync();
            dgvSinhVien.DataSource = null;
            dgvSinhVien.DataSource = danhSach;
            lblTrangThai.Text = $"Đã tải {danhSach.Count} sinh viên.";
            lblTrangThai.ForeColor = Color.DarkGreen;
        }
        catch (Exception ex)
        {
            lblTrangThai.Text = "Lỗi: " + ex.Message;
            lblTrangThai.ForeColor = Color.Red;
        }
    }

    // ===== Chọn dòng -> hiển thị lên ô nhập liệu =====
    private void DgvSinhVien_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvSinhVien.CurrentRow?.DataBoundItem is Student sv)
        {
            txtHoTen.Text = sv.FullName;
            txtDiem.Text = sv.Grade.ToString();
        }
    }

    // ===== CREATE =====
    private async void BtnThem_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
        {
            MessageBox.Show("Vui lòng nhập họ tên!");
            return;
        }
        if (!double.TryParse(txtDiem.Text, out double diem))
        {
            MessageBox.Show("Điểm không hợp lệ!");
            return;
        }

        try
        {
            using (var context = new AppDbContext())
            {
                context.Students.Add(new Student { FullName = txtHoTen.Text, Grade = diem });
                await context.SaveChangesAsync();
            }

            lblTrangThai.Text = "Đã thêm sinh viên mới!";
            lblTrangThai.ForeColor = Color.DarkGreen;
            txtHoTen.Clear();
            txtDiem.Clear();
            await TaiDanhSach();
        }
        catch (Exception ex)
        {
            lblTrangThai.Text = "Lỗi: " + ex.Message;
            lblTrangThai.ForeColor = Color.Red;
        }
    }

    // ===== UPDATE =====
    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        if (dgvSinhVien.CurrentRow?.DataBoundItem is not Student svDangChon)
        {
            MessageBox.Show("Vui lòng chọn một dòng để sửa!");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
        {
            MessageBox.Show("Vui lòng nhập họ tên!");
            return;
        }
        if (!double.TryParse(txtDiem.Text, out double diemMoi))
        {
            MessageBox.Show("Điểm không hợp lệ!");
            return;
        }

        try
        {
            using (var context = new AppDbContext())
            {
                Student? sv = await context.Students.FindAsync(svDangChon.Id);
                if (sv != null)
                {
                    sv.FullName = txtHoTen.Text;
                    sv.Grade = diemMoi;
                    await context.SaveChangesAsync();
                }
            }

            lblTrangThai.Text = "Đã cập nhật thông tin!";
            lblTrangThai.ForeColor = Color.DarkGreen;
            txtHoTen.Clear();
            txtDiem.Clear();
            await TaiDanhSach();
        }
        catch (Exception ex)
        {
            lblTrangThai.Text = "Lỗi: " + ex.Message;
            lblTrangThai.ForeColor = Color.Red;
        }
    }

    // ===== DELETE =====
    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        if (dgvSinhVien.CurrentRow?.DataBoundItem is not Student svDangChon)
        {
            MessageBox.Show("Vui lòng chọn một dòng để xóa!");
            return;
        }

        DialogResult ketQua = MessageBox.Show(
            $"Bạn có chắc muốn xóa \"{svDangChon.FullName}\"?",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (ketQua != DialogResult.Yes) return;

        try
        {
            using (var context = new AppDbContext())
            {
                Student? sv = await context.Students.FindAsync(svDangChon.Id);
                if (sv != null)
                {
                    context.Students.Remove(sv);
                    await context.SaveChangesAsync();
                }
            }

            lblTrangThai.Text = "Đã xóa sinh viên!";
            lblTrangThai.ForeColor = Color.DarkGreen;
            txtHoTen.Clear();
            txtDiem.Clear();
            await TaiDanhSach();
        }
        catch (Exception ex)
        {
            lblTrangThai.Text = "Lỗi: " + ex.Message;
            lblTrangThai.ForeColor = Color.Red;
        }
    }

    // ===== LINQ to Entities: Lọc sinh viên đạt =====
    private async void BtnLocDat_Click(object? sender, EventArgs e)
    {
        try
        {
            using var context = new AppDbContext();
            List<Student> ketQua = await context.Students
                .Where(sv => sv.Grade >= 5)
                .OrderByDescending(sv => sv.Grade)
                .ToListAsync();

            dgvSinhVien.DataSource = null;
            dgvSinhVien.DataSource = ketQua;
            lblTrangThai.Text = $"Tìm thấy {ketQua.Count} sinh viên đạt (>=5).";
            lblTrangThai.ForeColor = Color.DarkGreen;
        }
        catch (Exception ex)
        {
            lblTrangThai.Text = "Lỗi: " + ex.Message;
            lblTrangThai.ForeColor = Color.Red;
        }
    }
}
