using GeneratePocoTests.Supports;
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using System.ComponentModel.DataAnnotations;

namespace GeneratePocoTests;

public class ProductDtoTests
{
    [Fact]
    public void Rules()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;
using System.ComponentModel.DataAnnotations;

namespace GeneratePocoTests.Supports;

public class Product(int productId, string productName)
{
    public int ProductId { get; } = productId;
    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; } = productName;
}

[GeneratePoco<Product>(Rules = [""RemovePrefix Product"", ""Exclude: Id""])]
public partial class ProductDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>))
            .Reference<StringLengthAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.DoesNotContain("int Id", code);
        Assert.Contains("string Name", code);
    }
    [Fact]
    public void GenerateAttribute()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;
using System.ComponentModel.DataAnnotations;

namespace GeneratePocoTests.Supports;

public class Product(int productId, string productName)
{
    public int ProductId { get; } = productId;
    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; } = productName;
}

[GeneratePoco<Product>(GenerateAttribute = true)]
public partial class ProductDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>))
            .Reference<StringLengthAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("int ProductId", code);
        Assert.Contains("string ProductName", code);
    }
}

[GeneratePoco<Product>(Rules =
    [
        "RemovePrefix Product",
        "Exclude: Id"
    ],
    GenerateAttribute = true
    )]
public partial class ProductDto;
