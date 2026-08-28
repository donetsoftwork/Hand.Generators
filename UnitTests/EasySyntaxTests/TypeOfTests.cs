using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySyntaxTests;

public class TypeOfTests
{
    [Fact]
    public void Int()
    {
        var sourceText = "var type = typeof(int)";
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        var expression0 = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeOfExpressionSyntax>()
            .FirstOrDefault();
        Assert.NotNull(expression0);
        //SyntaxFactory.TypeOfExpression(SyntaxGenerator.IntType);
        var expression = SyntaxGenerator.IntType.TypeOf();
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("typeof(int)", code);
    }
    [Fact]
    public void Ilist()
    {
        var sourceText = "var type = typeof(Ilist<>)";
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        var expression0 = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeOfExpressionSyntax>()
            .FirstOrDefault();
        Assert.NotNull(expression0);
        //SyntaxFactory.TypeOfExpression(SyntaxGenerator.IntType);
        var expression = SyntaxGenerator.OmitGeneric("Ilist", 1).TypeOf();
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("typeof(Ilist<>)", code);
    }
    [Fact]
    public void Qualified()
    {
        var sourceText = "var type = typeof(System.Collections.Generic.Ilist<>)";
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        var expression0 = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeOfExpressionSyntax>()
            .FirstOrDefault();
        Assert.NotNull(expression0);
        //SyntaxFactory.TypeOfExpression(SyntaxGenerator.IntType);
        var expression = SyntaxGenerator.OmitGeneric("Ilist").Qualifies("System", "Collections", "Generic").TypeOf();
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("typeof(System.Collections.Generic.Ilist<>)", code);
    }
}
