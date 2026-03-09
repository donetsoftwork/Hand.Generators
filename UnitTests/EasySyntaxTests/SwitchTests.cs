using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Reflection.Emit;
using static Hand.SyntaxGenerator;

namespace EasySyntaxTests;

public class SwitchTests
{
    [Fact]
    public void IntToBool()
    {
        var value = SyntaxFactory.IdentifierName("value");
        // bool IntToBool(int value)
        var method = SyntaxGenerator.BoolType.Method("IntToBool", SyntaxGenerator.IntType.Parameter(value.Identifier))
            .ToBuilder()
            // switch(value){
            .Switch(value)
                // case 0:
                .Case(SyntaxGenerator.Literal(0))
                    // reurn false
                    .Add(SyntaxGenerator.FalseLiteral.Return())
                // case 1:
                .Case(SyntaxGenerator.Literal(1))
                    // reurn true
                    .Add(SyntaxGenerator.TrueLiteral.Return())
                // default:
                .Default()
                    // return true;
                    .Return(SyntaxGenerator.TrueLiteral)
            // }
            .End();

        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
        //Microsoft.CodeAnalysis.Editing.SyntaxGenerator
    }
    [Fact]
    public void IntToBool0()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)), "IntToBool")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(value.Identifier)
                    .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))))
            .AddBodyStatements(
                SyntaxFactory.SwitchStatement(value)
                    .AddSections(
                        SyntaxFactory.SwitchSection(
                            SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.CaseSwitchLabel(
                                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))),
                            SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ReturnStatement(
                                SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)))),
                        SyntaxFactory.SwitchSection(
                            SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.CaseSwitchLabel(
                                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))),
                            SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ReturnStatement(
                                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))),
                        SyntaxFactory.SwitchSection(
                            SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.DefaultSwitchLabel()),
                            SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ReturnStatement(
                                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))))
            );
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void IntToBool2()
    {
        var value = SyntaxFactory.IdentifierName("value");
        // bool IntToBool(int value)
        var method = BoolType.Method("IntToBool", IntType.Parameter(value.Identifier))
            .ToBuilder()
            // switch(value){
            .Switch(value)
                // case 0:
                .Case(Literal(0))
                    // reurn false
                    .Add(FalseLiteral.Return())
                // case 1:
                .Case(Literal(1))
                    // reurn true
                    .Add(TrueLiteral.Return())
                // default:
                .Default()
                    // return true;
                    .Return(TrueLiteral)
            // }
            .End();
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
}
