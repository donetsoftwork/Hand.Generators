using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection.Metadata;

namespace ReflectionTests;

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
    [Theory]
    [InlineData(SpecialType.System_Boolean)]
    [InlineData(SpecialType.System_Char)]
    [InlineData(SpecialType.System_SByte)]
    [InlineData(SpecialType.System_Int16)]
    [InlineData(SpecialType.System_Int32)]
    [InlineData(SpecialType.System_Int64)]
    [InlineData(SpecialType.System_Byte)]
    [InlineData(SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Single)]
    [InlineData(SpecialType.System_Double)]
    [InlineData(SpecialType.System_Decimal)]
    [InlineData(SpecialType.System_DateTime)]
    [InlineData(SpecialType.System_String)]
    [InlineData(SpecialType.System_Object)]
    public void GetNullable(SpecialType specialType)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var compilation = driver.Compile("");
        var symbol = compilation.GetSpecialType(specialType);
        var syntax = symbol.ToSyntax().ToFullString();
        var nullableSymbol = compilation.GetNullable(symbol);
        var nullableSyntax = nullableSymbol.ToSyntax().ToFullString();
        Assert.EndsWith("?", nullableSyntax);
        Assert.StartsWith(syntax, nullableSyntax);
        ITypeSymbol symbol2 = compilation.GetSpecialType(specialType);
        var syntax2 = symbol2.ToSyntax().ToFullString();
        ITypeSymbol nullableSymbol2 = compilation.GetNullable(symbol2);
        var nullableSyntax2 = nullableSymbol2.ToSyntax().ToFullString();
        Assert.EndsWith("?", nullableSyntax2);
        Assert.StartsWith(syntax2, nullableSyntax2);
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
