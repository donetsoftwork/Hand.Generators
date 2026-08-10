using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests;

public class TypeServicesTests
{
    [Theory]
    [InlineData("string")]
    [InlineData("string?")]
    [InlineData("int")]
    [InlineData("int?")]
    [InlineData("int[]")]
    [InlineData("System.Collections.Generic.IList<>")]
    [InlineData("System.Collections.Generic.List<int>")]
    public void ToSyntax(string typeName)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var syntax = SyntaxFactory.ParseTypeName(typeName);
        var compilation = driver.Compile("");
        var symbol = compilation.GetSymbol(syntax);
        Assert.NotNull(symbol);
        var syntax2 = symbol.ToSyntax();
        Assert.Equal(typeName, syntax2.ToFullString());
    }
    [Theory]
    [InlineData(SpecialType.System_Int32)]
    [InlineData(SpecialType.System_String)]
    [InlineData(SpecialType.System_Collections_Generic_IList_T)]
    public void SpecialTypeToSyntax(SpecialType specialType)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var compilation = driver.Compile("");
        var definitionType = compilation.GetSpecialType(specialType);
        var syntax = definitionType.ToSyntax();
        var symbolType = compilation.GetSymbol(syntax);
        Assert.NotNull(symbolType);
        Assert.True(definitionType.Equals(symbolType, SymbolEqualityComparer.Default));
    }
    [Fact]
    public void IsPrimitiveType()
    {
        Assert.False(SpecialType.System_Void.IsPrimitiveType());
        Assert.True(SpecialType.System_Boolean.IsPrimitiveType());
        Assert.True(SpecialType.System_String.IsPrimitiveType());        
        Assert.False(SpecialType.System_IntPtr.IsPrimitiveType());
        Assert.False(SpecialType.System_DateTime.IsPrimitiveType());
    }
    [Fact]
    public void IsIntegralType()
    {
        Assert.False(SpecialType.System_Boolean.IsIntegralType());
        Assert.True(SpecialType.System_SByte.IsIntegralType());
        Assert.True(SpecialType.System_UInt64.IsIntegralType());
        Assert.False(SpecialType.System_Decimal.IsIntegralType());
    }
    [Fact]
    public void IsNumericType()
    {
        Assert.False(SpecialType.System_Boolean.IsNumericType());
        Assert.True(SpecialType.System_SByte.IsNumericType());
        Assert.True(SpecialType.System_Double.IsNumericType());
        Assert.False(SpecialType.System_String.IsNumericType());
    }

    public static ITypeSymbol? GetTypeSymbol(Compilation compilation, SyntaxTree syntaxTree)
    {
        //var diagnostics = compilation.GetDiagnostics();
        //foreach (var diagnostic in diagnostics)
        //{
        //    Console.WriteLine(diagnostic.ToString());
        //}
        var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
        var type = GetSyntax<TypeSyntax>(semanticModel);
        Assert.NotNull(type);
        //var symbol = semanticModel.GetSymbolInfo(type);
        //return symbol.Symbol as INamedTypeSymbol;
        return compilation.GetSymbol(type);
    }
    /// <summary>
    /// 获取节点
    /// </summary>
    /// <typeparam name="TSyntax"></typeparam>
    /// <param name="semanticModel"></param>
    /// <returns></returns>
    public static TSyntax GetSyntax<TSyntax>(SemanticModel semanticModel)
        where TSyntax : TypeSyntax
    {
        var nodes = semanticModel.SyntaxTree
            .GetRoot()
            .DescendantNodes();
        var node = nodes.OfType<TSyntax>()
            .OrderByDescending(x => x.Span.Length)
            .FirstOrDefault();
        Assert.NotNull(node);
        return node;
    }
}
