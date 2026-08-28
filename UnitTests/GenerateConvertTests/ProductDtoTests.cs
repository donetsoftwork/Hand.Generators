using GenerateConvertTests.Supports;
using Hand;
using Hand.Mapping;

namespace GenerateConvertTests;

public class ProductDtoTests
{
    [Fact]
    public void Generate()
    {
        var source = @"
using GenerateConvertTests.Supports;
//using GenerateConvertTests.DTO;
using Hand.Mapping;

namespace GenerateConvertTests.DTO;

[GenerateConvert<Product>]
public partial class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public MyColorDTO ProductColor { get; set; }
    public UserDTO[] ProductUser { get; set; }
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference<Product>();
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToProduct", code);
    }
    [Fact]
    public void Prefix()
    {
        var source = @"
using GenerateConvertTests.Supports;
using GenerateConvertTests.DTO;
using Hand.Mapping;

namespace Tests;

[GenerateConvert<Product>(Rules = [""Prefix Product""])]
public partial class ProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public MyColorDTO Color { get; set; }
    public UserDTO ProductUser { get; set; }
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference<Product>();
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToProduct", code);
    }
}
