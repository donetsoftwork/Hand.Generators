using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using static Hand.SyntaxGenerator;

namespace EasySyntaxTests;

public class IfTest
{
    [Fact]
    public void BoolToString()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var method = SyntaxGenerator.StringType.Method("BoolToString", SyntaxGenerator.BoolType.Nullable().Parameter(value.Identifier))
            .ToBuilder()
            .If(value.IsNull())
                .Add(SyntaxGenerator.Literal("false").Return())
            .ElseIf(value)
                .Return(SyntaxGenerator.Literal("true"))
            .Return(SyntaxGenerator.Literal("false"));
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void BoolToString0()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "BoolToString")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(value.Identifier)
                    .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)))))
            .AddBodyStatements(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, value, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("false"))),
                    SyntaxFactory.ElseClause(
                        SyntaxFactory.IfStatement(
                        value,
                        SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("true"))),
                        SyntaxFactory.ElseClause(SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("false")))))))
            );

        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void BoolToString2()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var method = StringType.Method("BoolToString", BoolType.Nullable().Parameter(value.Identifier))
            .ToBuilder()
            .If(value.IsNull())
                .Add(Literal("false").Return())
            .ElseIf(value)
                .Return(Literal("true"))
            .Return(Literal("false"));
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
}
