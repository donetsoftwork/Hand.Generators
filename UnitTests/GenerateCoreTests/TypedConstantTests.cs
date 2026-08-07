using Hand;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenerateCoreTests;

public class TypedConstantTests
{
    [Fact]
    public void GetPrimitive()
    {
        string sourceCode = @"
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleNamespace;

[Table(""Products"")]
public record Product(int ProductId, string ProductName);
";
        var compilation = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<TableAttribute>()
            .Compile(sourceCode);
        var type = compilation.GetTypeByMetadataName("ExampleNamespace.Product");
        Assert.NotNull(type);
        var attribute = type.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, 0);
        Assert.NotNull(constant);
        var value = constant.Value.GetValue<string>();
        Assert.Equal("Products", value);
        var value0 = constant.Value.GetPrimitive<string>();
        Assert.Equal("Products", value0);
    }
    [Fact]
    public void GetEnum()
    {
        string sourceCode = @"
using System;

namespace ExampleNamespace;

[AttributeUsage(AttributeTargets.All)]
public class MyAttribute : Attribute;
";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var type = compilation.GetTypeByMetadataName("ExampleNamespace.MyAttribute");
        Assert.NotNull(type);
        var attribute = type.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, 0);
        Assert.NotNull(constant);
        var value = constant.Value.GetValue<AttributeTargets>();
        Assert.Equal(AttributeTargets.All, value);
        var value0 = constant.Value.GetEnum<AttributeTargets>();
        Assert.Equal(AttributeTargets.All, value0);
    }
    [Fact]
    public void GetValues()
    {
        string sourceCode = @"
using System;

namespace ExampleNamespace;

[AttributeUsage(AttributeTargets.Class)]
public class RecognizeAttribute : Attribute
{
    public string[] Rules { get; set; } = [];
}
public record Product(int Id, string Name);
[Recognize(Rules = [""Id:ProductId"", ""Name:ProductName""])]
public record ProductDto(int ProductId, string ProductName);
";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var type = compilation.GetTypeByMetadataName("ExampleNamespace.ProductDto");
        Assert.NotNull(type);
        var attribute = type.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, "Rules");
        Assert.NotNull(constant);
        var value = constant.Value.GetValue<string[]>();
        Assert.Equal(["Id:ProductId", "Name:ProductName"], value);
        var value0 = constant.Value.GetValues<string>();
        Assert.Equal(["Id:ProductId", "Name:ProductName"], value0);
    }
    [Fact]
    public void GetTypeSymbol()
    {
        string sourceCode = @"
using System;

namespace ExampleNamespace;

[AttributeUsage(AttributeTargets.Class)]
public class MapAttribute(Type from) : Attribute
{
    public Type From { get; } = from;
}
public record Product(int Id, string Name);
[Map(typeof(Product))]
public record ProductDto(int ProductId, string ProductName);
";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var type = compilation.GetTypeByMetadataName("ExampleNamespace.ProductDto");
        Assert.NotNull(type);
        var type0 = compilation.GetTypeByMetadataName("ExampleNamespace.Product");
        Assert.NotNull(type0);
        var attribute = type.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, 0);
        Assert.NotNull(constant);
        var value = constant.Value.GetValue<INamedTypeSymbol>();
        Assert.Equal(type0, value);
        var value0 = constant.Value.GetTypeSymbol();
        Assert.Equal(type0, value0);
    }
}
//public record Product(int Id, string Name);
//[Map(typeof(Product))]
//[Recognize(Rules = ["Id:ProductId", "Name:ProductName"])]
//public record ProductDto(int ProductId, string ProductName);
///// <summary>
///// 映射
///// </summary>
///// <param name="from"></param>
//[AttributeUsage(AttributeTargets.Class)]
//public class MapAttribute(Type from) : Attribute
//{
//    /// <summary>
//    /// 来源类型
//    /// </summary>
//    public Type From { get; } = from;
//}
///// <summary>
///// 映射
///// </summary>
//[AttributeUsage(AttributeTargets.Class)]
//public class RecognizeAttribute : Attribute
//{
//    /// <summary>
//    /// 规则
//    /// </summary>
//    public string[] Rules { get; set; } = [];
//}