using Microsoft.EntityFrameworkCore;
using QuanLySinhVienEF.Models;

namespace QuanLySinhVienEF.DAL;

/// <summary>
/// DAL – Triển khai thao tác CSDL cho Sản phẩm (async/await + EF Core)
/// </summary>
public class ProductRepository : IProductRepository
{
    // ── READ ────────────────────────────────────────────────────────────
    public async Task<List<Product>> GetAllAsync()
    {
        using var ctx = new AppDbContext();
        await ctx.KhoiTaoBangAsync();
        return await ctx.Products.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var ctx = new AppDbContext();
        return await ctx.Products.FindAsync(id);
    }

    // ── CREATE ──────────────────────────────────────────────────────────
    public async Task AddAsync(Product product)
    {
        using var ctx = new AppDbContext();
        ctx.Products.Add(product);
        await ctx.SaveChangesAsync();
    }

    // ── UPDATE ──────────────────────────────────────────────────────────
    public async Task UpdateAsync(Product product)
    {
        using var ctx = new AppDbContext();
        Product? existing = await ctx.Products.FindAsync(product.Id);
        if (existing is null) return;
        existing.Name     = product.Name;
        existing.Price    = product.Price;
        existing.Quantity = product.Quantity;
        await ctx.SaveChangesAsync();
    }

    // ── DELETE ──────────────────────────────────────────────────────────
    public async Task DeleteAsync(int id)
    {
        using var ctx = new AppDbContext();
        Product? p = await ctx.Products.FindAsync(id);
        if (p is null) return;
        ctx.Products.Remove(p);
        await ctx.SaveChangesAsync();
    }

    // ── LINQ to Entities: Lọc sản phẩm sắp hết hàng ───────────────────
    public async Task<List<Product>> GetLowStockAsync(int threshold = 10)
    {
        using var ctx = new AppDbContext();
        return await ctx.Products
                        .Where(p => p.Quantity < threshold)
                        .OrderBy(p => p.Quantity)
                        .ToListAsync();
    }
}
