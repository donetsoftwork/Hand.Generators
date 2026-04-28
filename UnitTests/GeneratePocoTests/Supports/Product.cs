using System.ComponentModel.DataAnnotations;

namespace GeneratePocoTests.Supports;

public class Product(int productId, string productName)
{
    public int ProductId { get; } = productId;
    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; } = productName;
}
