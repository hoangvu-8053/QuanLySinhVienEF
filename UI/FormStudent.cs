using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVienEF.BLL;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.UI;

/// <summary>
/// UI – Form quản lý Sinh viên (chỉ xử lý hiển thị, gọi BLL)
/// </summary>
public class FormStudent : Form
{
    // ── Lớp BLL được inject vào ────────────────────────────────────────
    private readonly StudentService _svc = new();

    // ── Controls ───────────────────────────────────────────────────────
    private readonly TextBox      txtHoTen  = new();
    private readonly TextBox      txtDiem   = new();
    private readonly Button       btnThem   = new();
    private readonly Button       btnSua    = new();
    private readonly Button       btnXoa    = new();
    private readonly Button       btnTaiLai = new();
    private readonly Button       btnLocDat = new();
    private readonly DataGridView dgv       = new();
    private readonly Label        lblStatus = new();

    public FormStudent()
    {
        Text          = "Quản lý Sinh viên — EF Core CRUD";
        ClientSize    = new Size(720, 570);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor     = Color.FromArgb(245, 247, 250);
        Font          = new Font("Segoe UI", 9.5f);

        XayDungGiaoDien();
        Load                 += async (_, _) => await TaiDanhSach();
        dgv.SelectionChanged += DgvSelectionChanged;
    }

    // ── Xây dựng giao diện ─────────────────────────────────────────────
    private void XayDungGiaoDien()
    {
        // Tiêu đề
        Controls.Add(new Label
        {
            Text      = "QUẢN LÝ SINH VIÊN — EF CORE (CRUD ĐẦY ĐỦ)",
            Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 60, 114),
            Location  = new Point(15, 14),
            AutoSize  = true
        });

        // Input panel
        var pnl = new Panel { Location = new Point(12, 50), Size = new Size(692, 72),
                              BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        Controls.Add(pnl);

        pnl.Controls.Add(NhanLabel("Họ tên *", new Point(10, 8)));
        txtHoTen.Location = new Point(10, 28); txtHoTen.Size = new Size(280, 26);
        txtHoTen.BorderStyle = BorderStyle.FixedSingle;
        pnl.Controls.Add(txtHoTen);

        pnl.Controls.Add(NhanLabel("Điểm (0–10) *", new Point(305, 8)));
        txtDiem.Location = new Point(305, 28); txtDiem.Size = new Size(130, 26);
        txtDiem.BorderStyle = BorderStyle.FixedSingle;
        pnl.Controls.Add(txtDiem);

        // Buttons
        var pnlBtn = new Panel { Location = new Point(12, 130), Size = new Size(692, 44),
                                 BackColor = Color.Transparent };
        Controls.Add(pnlBtn);

        CauHinhNut(btnThem,   "+ Thêm",            Color.FromArgb(34,139, 34),   0);
        CauHinhNut(btnSua,    "✏ Sửa",             Color.FromArgb(0, 112,200), 115);
        CauHinhNut(btnXoa,    "🗑 Xóa",             Color.FromArgb(196, 43, 28), 230);
        CauHinhNut(btnTaiLai, "🔄 Tải lại",         Color.FromArgb(100,100,100), 345);
        CauHinhNut(btnLocDat, "📊 SV đạt (LINQ)",   Color.FromArgb(130, 80,  0), 460);

        btnThem.Click   += BtnThem_Click;
        btnSua.Click    += BtnSua_Click;
        btnXoa.Click    += BtnXoa_Click;
        btnTaiLai.Click += async (_, _) => await TaiDanhSach();
        btnLocDat.Click += BtnLocDat_Click;

        foreach (var b in new[] { btnThem, btnSua, btnXoa, btnTaiLai, btnLocDat })
            pnlBtn.Controls.Add(b);

        // DataGridView
        CauHinhDgv();
        Controls.Add(dgv);

        // Status
        lblStatus.Location  = new Point(12, 542);
        lblStatus.Size      = new Size(692, 22);
        lblStatus.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStatus.ForeColor = Color.DimGray;
        Controls.Add(lblStatus);
    }

    private void CauHinhDgv()
    {
        dgv.Location    = new Point(12, 184);
        dgv.Size        = new Size(692, 348);
        dgv.Anchor      = AnchorStyles.Top | AnchorStyles.Bottom
                        | AnchorStyles.Left | AnchorStyles.Right;
        dgv.ReadOnly              = true;
        dgv.AllowUserToAddRows    = false;
        dgv.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect           = false;
        dgv.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.BackgroundColor       = Color.White;
        dgv.BorderStyle           = BorderStyle.None;
        dgv.RowHeadersVisible     = false;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 114);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        dgv.ColumnHeadersHeight   = 34;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 255);
        dgv.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(0, 112, 200);
        dgv.DefaultCellStyle.SelectionForeColor       = Color.White;
    }

    private static Label NhanLabel(string text, Point loc) =>
        new() { Text = text, Location = loc, AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80) };

    private static void CauHinhNut(Button btn, string text, Color bg, int x)
    {
        btn.Text      = text;
        btn.Location  = new Point(x, 5);
        btn.Size      = new Size(108, 34);
        btn.BackColor = bg;
        btn.ForeColor = Color.White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
        btn.Cursor    = Cursors.Hand;
    }

    private void SetTrangThai(string msg, Color? color = null)
    {
        lblStatus.Text      = msg;
        lblStatus.ForeColor = color ?? Color.DimGray;
    }

    private void XoaInputs() { txtHoTen.Clear(); txtDiem.Clear(); txtHoTen.Focus(); }

    // ── READ ────────────────────────────────────────────────────────────
    private async Task TaiDanhSach()
    {
        try
        {
            var list = await _svc.GetAllAsync();
            dgv.DataSource = null;
            dgv.DataSource = list;
            if (dgv.Columns.Contains("Id"))       dgv.Columns["Id"]!.HeaderText       = "ID";
            if (dgv.Columns.Contains("FullName")) dgv.Columns["FullName"]!.HeaderText = "Họ tên";
            if (dgv.Columns.Contains("Grade"))    dgv.Columns["Grade"]!.HeaderText    = "Điểm";
            SetTrangThai($"✔ Đã tải {list.Count} sinh viên.", Color.DarkGreen);
        }
        catch (Exception ex) { SetTrangThai("Lỗi: " + ex.Message, Color.Red); }
    }

    private void DgvSelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is Student sv)
        {
            txtHoTen.Text = sv.FullName;
            txtDiem.Text  = sv.Grade.ToString();
        }
    }

    // ── CREATE ──────────────────────────────────────────────────────────
    private async void BtnThem_Click(object? sender, EventArgs e)
    {
        var (ok, msg) = await _svc.AddAsync(txtHoTen.Text, txtDiem.Text);
        SetTrangThai(msg, ok ? Color.DarkGreen : Color.OrangeRed);
        if (ok) { XoaInputs(); await TaiDanhSach(); }
    }

    // ── UPDATE ──────────────────────────────────────────────────────────
    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not Student sv)
        {
            SetTrangThai("⚠ Vui lòng chọn sinh viên cần sửa!", Color.OrangeRed); return;
        }
        var (ok, msg) = await _svc.UpdateAsync(sv.Id, txtHoTen.Text, txtDiem.Text);
        SetTrangThai(msg, ok ? Color.DarkGreen : Color.OrangeRed);
        if (ok) { XoaInputs(); await TaiDanhSach(); }
    }

    // ── DELETE ──────────────────────────────────────────────────────────
    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not Student sv)
        {
            SetTrangThai("⚠ Vui lòng chọn sinh viên cần xóa!", Color.OrangeRed); return;
        }
        if (MessageBox.Show($"Xóa \"{sv.FullName}\"?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        var (ok, msg) = await _svc.DeleteAsync(sv.Id, sv.FullName);
        SetTrangThai(msg, ok ? Color.DarkGreen : Color.OrangeRed);
        if (ok) { XoaInputs(); await TaiDanhSach(); }
    }

    // ── LINQ — Lọc sinh viên đạt (điểm >= 5) ──────────────────────────
    private async void BtnLocDat_Click(object? sender, EventArgs e)
    {
        try
        {
            var list = await _svc.GetPassedAsync();
            dgv.DataSource = null;
            dgv.DataSource = list;
            SetTrangThai($"✔ {list.Count} sinh viên đạt (Điểm ≥ 5).", Color.DarkGreen);
        }
        catch (Exception ex) { SetTrangThai("Lỗi: " + ex.Message, Color.Red); }
    }
}
