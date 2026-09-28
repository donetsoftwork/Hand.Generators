using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class ArgumentTests
{
    [Fact]
    public void Literal()
    {
        ArgumentSyntax literal = SyntaxGenerator.Literal(1)
            .ToArgument();
        Assert.Equal("1", literal.ToFullString());
    }
    [Fact]
    public void Literal0()
    {
        ArgumentSyntax literal = SyntaxFactory.Argument(
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
            SyntaxFactory.Literal(1)));
        Assert.Equal("1", literal.ToFullString());
    }
    [Fact]
    public void Expression()
    {
        ArgumentSyntax expression = SyntaxFactory.IdentifierName("reader")
            .Access("Read")
            .Invocation()
            .ToArgument();
        Assert.Equal("reader.Read()", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Expression0()
    {
        ArgumentSyntax expression = SyntaxFactory.Argument(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression, 
                    SyntaxFactory.IdentifierName("reader"), 
                    SyntaxFactory.IdentifierName("Read")), 
                SyntaxFactory.ArgumentList()));
        Assert.Equal("reader.Read()", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Ref()
    {
        ArgumentSyntax value = SyntaxFactory.IdentifierName("value")
            .ToArgument()
            .Ref();
        Assert.Equal("ref value", value.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Ref0()
    {
        ArgumentSyntax value = SyntaxFactory.Argument(
            SyntaxFactory.IdentifierName("value")
        ).WithRefKindKeyword(
            SyntaxFactory.Token(SyntaxKind.RefKeyword));
        Assert.Equal("ref value", value.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void OutArgument()
    {
        ArgumentSyntax outArgument = SyntaxGenerator.VarType.OutArgument("value");
        Assert.True(outArgument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword));
        Assert.Equal("out var value", outArgument.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void OutArgument0()
    {
        ArgumentSyntax outArgument = SyntaxFactory.Argument(
            default, 
            SyntaxFactory.Token(SyntaxKind.OutKeyword), 
            SyntaxFactory.DeclarationExpression(
                SyntaxFactory.IdentifierName("var"), 
                SyntaxFactory.SingleVariableDesignation(
                    SyntaxFactory.Identifier("value"))));
        Assert.True(outArgument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword));
        Assert.Equal("out var value", outArgument.NormalizeWhitespace().ToFullString());
    }
}
