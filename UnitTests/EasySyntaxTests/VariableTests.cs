using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class VariableTests
{
    [Fact]
    public void Singleton()
    {
        var x = SyntaxGenerator.IntType.Variable("x");
        Assert.Equal("int x", x.NormalizeWhitespace().ToFullString());
        var y = SyntaxGenerator.IntType.Variable("y", SyntaxGenerator.Literal(1));
        Assert.Equal("int y = 1", y.NormalizeWhitespace().ToFullString());
        var z = SyntaxGenerator.VarType.Variable("z", SyntaxGenerator.Literal(1));
        Assert.Equal("var z = 1", z.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Singleton0()
    {
        var x = SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
            .AddVariables(SyntaxFactory.VariableDeclarator("x"));
        Assert.Equal("int x", x.NormalizeWhitespace().ToFullString());
        var y = SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
            .AddVariables(SyntaxFactory.VariableDeclarator("y")
                .WithInitializer(SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))));
        Assert.Equal("int y = 1", y.NormalizeWhitespace().ToFullString());
        var z = SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
            .AddVariables(SyntaxFactory.VariableDeclarator("z")
                .WithInitializer(SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))));
        Assert.Equal("var z = 1", z.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void List()
    {
        int x = 1, y = 1;
        Assert.Equal(x, y);
    }
}
