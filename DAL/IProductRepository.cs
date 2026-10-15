using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.DAL;

/// <summary>
/// DAL – Interface định nghĩa các thao tác dữ liệu cho Sản phẩm
/// </summary>
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?>      GetByIdAsync(int id);
    Task                AddAsync(Product product);
    Task                UpdateAsync(Product product);
    Task                DeleteAsync(int id);
    Task<List<Product>> GetLowStockAsync(int threshold = 10); // LINQ: sắp hết hàng
}
