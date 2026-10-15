using QuanLySinhVienEF.DAL;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.BLL;

/// <summary>
/// BLL – Xử lý nghiệp vụ Sản phẩm: validate đầu vào + điều phối DAL
/// </summary>
public class ProductService
{
    private readonly IProductRepository _repo;

    // Constructor mặc định (dùng trong UI)
    public ProductService() : this(new ProductRepository()) { }

    // Constructor inject (dùng cho unit test)
    public ProductService(IProductRepository repo) => _repo = repo;

    // ── READ ────────────────────────────────────────────────────────────
    public Task<List<Product>> GetAllAsync()      => _repo.GetAllAsync();
    public Task<List<Product>> GetLowStockAsync() => _repo.GetLowStockAsync();

    // ── CREATE ──────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Message)> AddAsync(
        string tenSP, string giaText, string slText)
    {
        var (ok, name, price, qty, err) = Validate(tenSP, giaText, slText);
        if (!ok) return (false, err);

        await _repo.AddAsync(new Product { Name = name, Price = price, Quantity = qty });
        return (true, $"✔ Đã thêm sản phẩm \"{name}\".");
    }

    // ── UPDATE ──────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Message)> UpdateAsync(
        int id, string tenSP, string giaText, string slText)
    {
        var (ok, name, price, qty, err) = Validate(tenSP, giaText, slText);
        if (!ok) return (false, err);

        await _repo.UpdateAsync(new Product { Id = id, Name = name, Price = price, Quantity = qty });
        return (true, $"✔ Đã cập nhật sản phẩm \"{name}\".");
    }

    // ── DELETE ──────────────────────────────────────────────────────────
    public async Task<(bool Ok, string Message)> DeleteAsync(int id, string tenSP)
    {
        await _repo.DeleteAsync(id);
        return (true, $"✔ Đã xóa sản phẩm \"{tenSP}\".");
    }

    // ── Validate dữ liệu đầu vào ────────────────────────────────────────
    private static (bool Ok, string Name, decimal Price, int Qty, string Error)
        Validate(string tenSP, string giaText, string slText)
    {
        string name = tenSP.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return (false, "", 0, 0, "⚠ Tên sản phẩm không được để trống!");

        if (!decimal.TryParse(giaText.Trim(), out decimal price) || price < 0)
            return (false, "", 0, 0, "⚠ Giá phải là số không âm!");

        if (!int.TryParse(slText.Trim(), out int qty) || qty < 0)
            return (false, "", 0, 0, "⚠ Số lượng phải là số nguyên không âm!");

        return (true, name, price, qty, "");
    }
}
