using GeneratePocoTests.Supports;
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using System.ComponentModel.DataAnnotations;

namespace GeneratePocoTests;

public class ProductDtoTests
{
    [Fact]
    public void GenerateWithRules()
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

[GeneratePoco(typeof(Product), Rules = [""RemovePrefix Product"", ""Exclude: Id""], Color = ConsoleColor.Red)]
public partial class ProductDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>()
            .Reference<StringLengthAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.DoesNotContain("Id", code);
        Assert.Contains("Name", code);
    }
}

[GeneratePoco(typeof(Product) , 
    Rules =
    [
        "RemovePrefix Product",
        "Exclude: Id"
    ]
    )]
public partial class ProductDto;
