using Hand;
using Hand.Builders;
using Microsoft.CodeAnalysis.CSharp;

namespace TypesTests;

public class TypeOfTests
{
    [Theory]
    [InlineData("string", "typeof(string)")]
    [InlineData("string?", "typeof(string)")]
    [InlineData("int", "typeof(int)")]
    [InlineData("int?", "typeof(int?)")]
    [InlineData("int[]", "typeof(int[])")]
    [InlineData("System.Collections.Generic.IList<>", "typeof(IList<>)")]
    [InlineData("System.Collections.Generic.List<int>", "typeof(List<int>)")]
    public void Display(string typeName, string expected)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var syntax = SyntaxFactory.ParseTypeName(typeName);
        var compilation = driver.Compile("");
        var symbol = compilation.GetSymbol(syntax);
        Assert.NotNull(symbol);
        var infos = new TypeInfoBuilder(compilation);
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var info = infos.Get(symbol);
        var result = info.TypeOf(generator);
        Assert.Equal(expected, result.ToFullString());
    }
}
