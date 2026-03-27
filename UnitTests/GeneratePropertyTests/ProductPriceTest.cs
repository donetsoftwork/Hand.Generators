using Hand;
using Hand.Entities;
using Hand.GenerateProperty;
using Hand.Models;

namespace GeneratePropertyTests;

public class ProductPriceTest
{
    //[Fact]
    //public void Test()
    //{
    //    //var type = typeof(IEntityProperty<>);
    //    var value = 9.9M;
    //    var price = new ProductPrice(value);
    //    Assert.Equal(value, price.Original);
    //    var price2 = new ProductPrice(9.9M);
    //    Assert.Equal(price, price2);
    //    Assert.True(price == price2);
    //}
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand.Entities;
using Hand.Models;
namespace GeneratePropertyTests;

[GenerateProperty(""Include: Constructor"")]
public partial record struct ProductPrice : IEntityProperty<decimal>;
";
        var service = SyntaxTreeScript.CreateDefault()
            .Reference<IEntityProperty<decimal>>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate<PropertyGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Original", code);
    }
}

//[GenerateProperty("Include: Constructor")]
//public partial record struct ProductPrice : IEntityProperty<decimal>;