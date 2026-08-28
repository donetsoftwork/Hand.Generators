using Hand;
using Hand.Cachers;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateCoreTests;

public class DisplayTests
{
    [Theory]
    [InlineData("string", "string")]
    [InlineData("string?", "string?")]
    [InlineData("int", "int")]
    [InlineData("int?", "int?")]
    [InlineData("int[]", "int[]")]
    [InlineData("System.Collections.Generic.IList<>", "IList<>")]
    [InlineData("System.Collections.Generic.List<int>", "List<int>")]
    public void Display(string typeName, string expected)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var syntax = SyntaxFactory.ParseTypeName(typeName);
        var compilation = driver.Compile("");
        var symbol = compilation.GetSymbol(syntax);
        Assert.NotNull(symbol);
        var infos = new TypeSymbolCacher(compilation);
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var info = infos.Get(symbol);
        var displayName = generator.Display(info);
        Assert.Equal(expected, displayName.ToFullString());
    }
    //[Theory]
    //[InlineData("string", "string", "typeof(string)")]
    //[InlineData("string?", "string?", "typeof(string)")]
    //[InlineData("int", "int", "typeof(int)")]
    //[InlineData("int?", "int?", "typeof(int?)")]
    //[InlineData("int[]", "int[]", "typeof(int[])")]
    //[InlineData("System.Collections.Generic.IList<>", "IList<T>", "typeof(IList<>)")]
    //[InlineData("System.Collections.Generic.List<int>", "List<int>", "typeof(List<int>)")]
    //public void TypeOf(string typeName, string expected, string expected2)
    //{
    //    var driver = SyntaxTreeDriver.CreateDriver()
    //        .Reference<object>();
    //    var syntax = SyntaxFactory.ParseTypeName(typeName);
    //    var compilation = driver.Compile("");
    //    var symbol = compilation.GetSymbol(syntax);
    //    Assert.NotNull(symbol);
    //    var infos = new TypeSymbolCacher(compilation);
    //    var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
    //    var info = infos.Get(symbol);
    //    var displayName = generator.Display(info);
    //    Assert.Equal(expected, displayName.ToFullString());
    //    var typeofResult = displayName.TypeOf();
    //    Assert.Equal(expected2, typeofResult.ToFullString());
    //}
}
