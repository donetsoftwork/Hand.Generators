using GeneratePropertyTests.Sources;
using Hand;
using Hand.Entities;
using Hand.Filters;
using Hand.GenerateProperty;
using Hand.Generators;
using Hand.Models;
using Hand.Transform;
using Microsoft.CodeAnalysis;

namespace GeneratePropertyTests;

public class UserNameTests
{
    //[Fact]
    //public void Test()
    //{
    //    var value = "Jxj";
    //    var userName = new UserName(value);
    //    Assert.Equal(value, userName.Original);
    //    var userName2 = new UserName("Jxj");
    //    Assert.Equal(userName, userName2);
    //    Assert.True(userName == userName2);
    //    //// 不可访问
    //    //var userName3 = new UserName(userName);
    //    //var userName3 = userName with { };
    //}
    //[Fact]
    //public void NotEqual()
    //{
    //    var userName = new UserName("Jxj");
    //    var userName2 = new UserName("Jxj2");
    //    Assert.NotEqual(userName, userName2);
    //    Assert.True(userName != userName2);
    //    Assert.False(userName == userName2);
    //}    
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand.Entities;
using Hand.Models;
namespace GeneratePropertyTests;

[GenerateProperty]
public partial record UserName : IEntityProperty<string>;
";
        var service = SyntaxTreeScript.Create()
            .Reference<IEntityProperty<string>>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate<PropertyGenerator>(source, out var diagnostics);
        var syntaxTree = result.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("UserName(string Original)", code);
        Assert.Contains("ToString()", code);
        Assert.Contains("GetHashCode()", code);
    }
    [Fact]
    public void SourceText()
    {
        var source = @"
using Hand.Entities;
using Hand.Models;
namespace GeneratePropertyTests;

[GenerateProperty]
public partial record UserName : IEntityProperty<string>;
";
        var generator = new ValuesGenerator<GeneratorAttributeSyntaxContext>("Hand.Entities.GeneratePropertyAttribute", new SyntaxFilter(), PassTransform.Instance, new SourceTextExecutor());
        var service = SyntaxTreeScript.Create()
            .Reference<IEntityProperty<string>>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate(generator, source, out var diagnostics);
        var syntaxTree = result.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("UserName(string Original)", code);
        Assert.Contains("ToString()", code);
        Assert.Contains("GetHashCode()", code);
    }
}
//[GenerateProperty]
//public partial record UserName : IEntityProperty<string>;
