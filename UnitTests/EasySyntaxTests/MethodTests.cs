using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class MethodTests
{
    [Fact]
    public void Increment()
    {
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "Increment")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("num"))
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))),
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
                    .WithDefault(SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))))
            )
            .WithBody(SyntaxFactory.Block(
                SyntaxFactory.ReturnStatement(
                    SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                        SyntaxFactory.IdentifierName("num"),
                        SyntaxFactory.IdentifierName("value"))
                )
            ));
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Increment2()
    {
        var num = SyntaxFactory.IdentifierName("num");
        var value = SyntaxFactory.IdentifierName("value");
        var method = SyntaxGenerator.IntType.Method("Increment", 
                SyntaxGenerator.IntType.Parameter(num.Identifier), 
                SyntaxGenerator.IntType.Parameter(value.Identifier, SyntaxGenerator.Literal(1)))
            .ToBuilder()
            .Return(num.Add(value));
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
}
