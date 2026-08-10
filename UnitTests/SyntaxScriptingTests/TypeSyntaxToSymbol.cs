using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxScriptingTests;

public class TypeSyntaxToSymbol
{
    [Fact]
    public void PredefinedType()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var intType = SyntaxGenerator.IntType;
        INamedTypeSymbol symbol = compilation.GetSymbol(intType);
        Assert.Equal(SpecialType.System_Int32, symbol.SpecialType);
        INamedTypeSymbol symbol2 = compilation.GetIntSymbol();
        Assert.Equal(SpecialType.System_Int32, symbol2.SpecialType);
    }
    [Fact]
    public void Nullable()
    {
        NullableTypeSyntax nullableSyntax = SyntaxFactory.NullableType(SyntaxGenerator.IntType);
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        INamedTypeSymbol? symbol = compilation.GetSymbol(nullableSyntax);
        Assert.NotNull(symbol);
        var nullableSymbol = compilation.GetSpecialType(SpecialType.System_Nullable_T);
        Assert.True(symbol.IsGenericType(nullableSymbol));
    }
    [Fact]
    public void Array()
    {
        ArrayTypeSyntax arraySyntax = SyntaxGenerator.IntType.Array();
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        IArrayTypeSymbol? arraySymbol = compilation.GetSymbol(arraySyntax);
        Assert.NotNull(arraySymbol);
    }
    [Fact]
    public void Generic()
    {
        GenericNameSyntax genericSyntax = SyntaxGenerator.Generic("System.Collections.Generic.IList", SyntaxGenerator.IntType);
        var service = SyntaxTreeDriver.CreateDriver()
             .Reference<object>();
        var compilation = service.Compile("");
        if(compilation.GetSymbol(genericSyntax) is not INamedTypeSymbol genericSymbol)
        {
            Assert.Fail();
            return;
        }
        var listSymbol = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
        Assert.True(genericSymbol.IsGenericType(listSymbol));
    }
}
