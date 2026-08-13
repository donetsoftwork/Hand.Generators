using Hand.Sql;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneratePocoTests.Supports;

[Table("Products")]
public class Product(int productId, string productName)
{
    [Key, Unique]
    public int ProductId { get; } = productId;
    [Unique]
    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; } = productName;
}
