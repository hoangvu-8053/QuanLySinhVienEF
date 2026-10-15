using System.Drawing;
using System.Windows.Forms;
using QuanLySinhVienEF.BLL;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.UI;

/// <summary>
/// UI – Form quản lý Sản phẩm trong kho (chỉ xử lý hiển thị, gọi BLL)
/// </summary>
public class FormProduct : Form
{
    // ── Lớp BLL được inject vào ────────────────────────────────────────
    private readonly ProductService _svc = new();

    // ── Controls ───────────────────────────────────────────────────────
    private readonly TextBox      txtName   = new();
    private readonly TextBox      txtPrice  = new();
    private readonly TextBox      txtQty    = new();
    private readonly Button       btnThem   = new();
    private readonly Button       btnSua    = new();
    private readonly Button       btnXoa    = new();
    private readonly Button       btnTaiLai = new();
    private readonly Button       btnSapHet = new();
    private readonly DataGridView dgv       = new();
    private readonly Label        lblStatus = new();

    public FormProduct()
    {
        Text          = "Quản lý Sản phẩm Kho — EF Core Code First";
        ClientSize    = new Size(800, 590);
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
            Text      = "📦  QUẢN LÝ SẢN PHẨM TRONG KHO",
            Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 60, 114),
            Location  = new Point(15, 14),
            AutoSize  = true
        });

        // Input panel
        var pnl = new Panel { Location = new Point(12, 50), Size = new Size(772, 72),
                              BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        Controls.Add(pnl);

        pnl.Controls.Add(NhanLabel("Tên sản phẩm *", new Point(10, 8)));
        txtName.Location     = new Point(10, 28); txtName.Size = new Size(310, 26);
        txtName.BorderStyle  = BorderStyle.FixedSingle;
        pnl.Controls.Add(txtName);

        pnl.Controls.Add(NhanLabel("Giá (VNĐ) *", new Point(335, 8)));
        txtPrice.Location    = new Point(335, 28); txtPrice.Size = new Size(170, 26);
        txtPrice.BorderStyle = BorderStyle.FixedSingle;
        pnl.Controls.Add(txtPrice);

        pnl.Controls.Add(NhanLabel("Số lượng *", new Point(520, 8)));
        txtQty.Location      = new Point(520, 28); txtQty.Size = new Size(130, 26);
        txtQty.BorderStyle   = BorderStyle.FixedSingle;
        pnl.Controls.Add(txtQty);

        // Button panel
        var pnlBtn = new Panel { Location = new Point(12, 130), Size = new Size(772, 44),
                                 BackColor = Color.Transparent };
        Controls.Add(pnlBtn);

        CauHinhNut(btnThem,   "+ Thêm",                   Color.FromArgb(34,139, 34),   0);
        CauHinhNut(btnSua,    "✏ Sửa",                    Color.FromArgb(0, 112,200), 120);
        CauHinhNut(btnXoa,    "🗑 Xóa",                    Color.FromArgb(196, 43, 28), 240);
        CauHinhNut(btnTaiLai, "🔄 Tải lại",                Color.FromArgb(100,100,100), 360);
        CauHinhNut(btnSapHet, "⚠ Sắp hết hàng (LINQ)",   Color.FromArgb(180,100,  0), 480, 180);

        btnThem.Click   += BtnThem_Click;
        btnSua.Click    += BtnSua_Click;
        btnXoa.Click    += BtnXoa_Click;
        btnTaiLai.Click += async (_, _) => await TaiDanhSach();
        btnSapHet.Click += BtnSapHet_Click;

        foreach (var b in new[] { btnThem, btnSua, btnXoa, btnTaiLai, btnSapHet })
            pnlBtn.Controls.Add(b);

        // DataGridView
        CauHinhDgv();
        Controls.Add(dgv);

        // Status
        lblStatus.Location  = new Point(12, 558);
        lblStatus.Size      = new Size(772, 22);
        lblStatus.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStatus.ForeColor = Color.DimGray;
        Controls.Add(lblStatus);
    }

    private void CauHinhDgv()
    {
        dgv.Location    = new Point(12, 184);
        dgv.Size        = new Size(772, 364);
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

    private static void CauHinhNut(Button btn, string text, Color bg, int x, int w = 108)
    {
        btn.Text      = text;
        btn.Location  = new Point(x, 5);
        btn.Size      = new Size(w, 34);
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

    private void XoaInputs() { txtName.Clear(); txtPrice.Clear(); txtQty.Clear(); txtName.Focus(); }

    // ── READ ────────────────────────────────────────────────────────────
    private async Task TaiDanhSach()
    {
        try
        {
            var list = await _svc.GetAllAsync();
            HienThiLuoi(list);
            SetTrangThai($"✔ Đã tải {list.Count} sản phẩm.", Color.DarkGreen);
        }
        catch (Exception ex) { SetTrangThai("Lỗi: " + ex.Message, Color.Red); }
    }

    private void HienThiLuoi(List<Product> list)
    {
        dgv.DataSource = null;
        dgv.DataSource = list;

        if (dgv.Columns.Contains("Id"))       dgv.Columns["Id"]!.HeaderText       = "ID";
        if (dgv.Columns.Contains("Name"))     dgv.Columns["Name"]!.HeaderText     = "Tên sản phẩm";
        if (dgv.Columns.Contains("Price"))    dgv.Columns["Price"]!.HeaderText    = "Giá (VNĐ)";
        if (dgv.Columns.Contains("Quantity")) dgv.Columns["Quantity"]!.HeaderText = "Số lượng";

        // Tô màu cảnh báo các hàng sắp hết (Quantity < 10)
        foreach (DataGridViewRow row in dgv.Rows)
        {
            if (row.DataBoundItem is Product p && p.Quantity < 10)
            {
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                row.DefaultCellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
        }
    }

    private void DgvSelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is Product p)
        {
            txtName.Text  = p.Name;
            txtPrice.Text = p.Price.ToString();
            txtQty.Text   = p.Quantity.ToString();
        }
    }

    // ── CREATE ──────────────────────────────────────────────────────────
    private async void BtnThem_Click(object? sender, EventArgs e)
    {
        var (ok, msg) = await _svc.AddAsync(txtName.Text, txtPrice.Text, txtQty.Text);
        SetTrangThai(msg, ok ? Color.DarkGreen : Color.OrangeRed);
        if (ok) { XoaInputs(); await TaiDanhSach(); }
    }

    // ── UPDATE ──────────────────────────────────────────────────────────
    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not Product p)
        {
            SetTrangThai("⚠ Vui lòng chọn sản phẩm cần sửa!", Color.OrangeRed); return;
        }
        var (ok, msg) = await _svc.UpdateAsync(p.Id, txtName.Text, txtPrice.Text, txtQty.Text);
        SetTrangThai(msg, ok ? Color.DarkGreen : Color.OrangeRed);
        if (ok) { XoaInputs(); await TaiDanhSach(); }
    }

    // ── DELETE ──────────────────────────────────────────────────────────
    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not Product p)
        {
            SetTrangThai("⚠ Vui lòng chọn sản phẩm cần xóa!", Color.OrangeRed); return;
        }
        if (MessageBox.Show($"Xóa \"{p.Name}\"?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        var (ok, msg) = await _svc.DeleteAsync(p.Id, p.Name);
        SetTrangThai(msg, ok ? Color.DarkGreen : Color.OrangeRed);
        if (ok) { XoaInputs(); await TaiDanhSach(); }
    }

    // ── LINQ — Lọc sản phẩm sắp hết hàng (Quantity < 10) ──────────────
    private async void BtnSapHet_Click(object? sender, EventArgs e)
    {
        try
        {
            var list = await _svc.GetLowStockAsync();
            HienThiLuoi(list);
            SetTrangThai(
                list.Count == 0
                    ? "✔ Không có sản phẩm nào sắp hết hàng."
                    : $"⚠ Có {list.Count} sản phẩm sắp hết hàng (Số lượng < 10).",
                list.Count == 0 ? Color.DarkGreen : Color.DarkOrange);
        }
        catch (Exception ex) { SetTrangThai("Lỗi: " + ex.Message, Color.Red); }
    }
}
