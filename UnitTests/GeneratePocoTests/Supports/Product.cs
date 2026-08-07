using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneratePocoTests.Supports;

public class Product(int productId, string productName)
{
    [Column("ProductId")]
    public int ProductId { get; } = productId;
    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; } = productName;
}
