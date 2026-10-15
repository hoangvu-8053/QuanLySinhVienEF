using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVienEF.Models;

/// <summary>
/// Entity: Sản phẩm trong kho
/// </summary>
public class Product
{
    public int     Id       { get; set; }

    [Required]
    public string  Name     { get; set; } = string.Empty;

    public decimal Price    { get; set; }

    public int     Quantity { get; set; }
}
