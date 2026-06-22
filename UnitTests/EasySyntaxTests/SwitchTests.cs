using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    [Fact]
    public void IntToBool3()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var body = value.Switch()
            .Case(Literal(0))
                .Add(FalseLiteral.Return())
            .Case(Literal(1))
                .Add(TrueLiteral.Return())
            .Default()
                .Add(TrueLiteral.Return())
            .Block();


        // bool IntToBool(int value)
        var method = BoolType.Method("IntToBool", IntType.Parameter(value.Identifier))
            .WithBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //bool IntToBool(int value)
    //{
    //    switch (value)
    //    {
    //        case 0:
    //            return false;
    //        case 1:
    //            return true;
    //        default:
    //            return true;
    //    }
    //}
    [Fact]
    public void ScoreToGrade()
    {
        var score = SyntaxFactory.IdentifierName("score");
        var body = score.Switch()
            .Case(SyntaxGenerator.Literal(10), SyntaxGenerator.Literal(9))
                .Add(SyntaxGenerator.Literal("优").Return())
            .Case(Literal(8), Literal(7))
                .Add(SyntaxGenerator.Literal("良").Return())
            .Case(Literal(6))
                .Add(SyntaxGenerator.Literal("中").Return())
            .Default()
                .Add(SyntaxGenerator.Literal("差").Return())
            .Block();
        // bool IntToBool(int value)
        var method = SyntaxGenerator.StringType.Method("ScoreToGrade", SyntaxGenerator.IntType.Parameter(score.Identifier))
            .WithBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //string ScoreToGrade(int score)
    //{
    //    switch (score)
    //    {
    //        case 10:
    //        case 9:
    //            return "优";
    //        case 8:
    //        case 7:
    //            return "良";
    //        case 6:
    //            return "中";
    //        default:
    //            return "差";
    //    }
    //}
}
