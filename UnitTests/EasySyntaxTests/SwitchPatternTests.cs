using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class SwitchPatternTests
{
    [Fact]
    public void IntToBool0()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)), "IntToBool")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(value.Identifier)
                    .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))))
            .AddBodyStatements(
                SyntaxFactory.ReturnStatement(
                    SyntaxFactory.SwitchExpression(value)
                        .AddArms(
                            SyntaxFactory.SwitchExpressionArm(
                                SyntaxFactory.ConstantPattern(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0))),
                                SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)),
                            SyntaxFactory.SwitchExpressionArm(
                                SyntaxFactory.ConstantPattern(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))),
                                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)),
                            SyntaxFactory.SwitchExpressionArm(
                                SyntaxFactory.DiscardPattern(),
                                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
                        )
                )
            );
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //bool IntToBool(int? value)
    //{
    //    return value switch
    //    {
    //        0 => false,
    //        1 => true,
    //        _ => true
    //    };
    //}
    [Fact]
    public void IntToBool()
    {
        var value = SyntaxFactory.IdentifierName("value");
        var body = value.SwitchExpression()
            .Case(SyntaxGenerator.Literal(0), SyntaxGenerator.FalseLiteral)
            .Case(SyntaxGenerator.Literal(1), SyntaxGenerator.TrueLiteral)
            .Default(SyntaxGenerator.TrueLiteral)
            .Build();
        var method = SyntaxGenerator.BoolType.Method("IntToBool", SyntaxGenerator.IntType.Parameter(value.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //public static bool IntToBool(int value) => value switch
    //{
    //    0 => false,
    //    1 => true,
    //    _ => true
    //};
    [Fact]
    public void WhatFruit()
    {
        var fruitType = SyntaxFactory.IdentifierName("Fruit");
        var fruit = SyntaxFactory.IdentifierName("fruit");
        var appleType = SyntaxFactory.IdentifierName("Apple");
        var body = fruit.SwitchExpression()
            .Case(appleType.DiscardPattern(), SyntaxGenerator.Literal("This is an apple"))
            .Default(SyntaxGenerator.Literal("This is not an apple"))
            .Build();
        var method = SyntaxGenerator.StringType.Method("WhatFruit", fruitType.Parameter(fruit.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //string WhatFruit(Fruit fruit) => fruit switch
    //{
    //    Apple _ => "This is an apple",
    //    _ => "This is not an apple"
    //};
    [Fact]
    public void WhatFruit0()
    {
        var fruitType = SyntaxFactory.IdentifierName("Fruit");
        var fruit = SyntaxFactory.IdentifierName("fruit");
        var appleType = SyntaxFactory.IdentifierName("Apple");

        var body = SyntaxFactory.SwitchExpression(fruit, SyntaxFactory.SeparatedList([
                SyntaxFactory.SwitchExpressionArm(
                    SyntaxFactory.DeclarationPattern(appleType, SyntaxFactory.DiscardDesignation()), 
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, 
                        SyntaxFactory.Literal("This is an apple"))),
                SyntaxFactory.SwitchExpressionArm(
                    SyntaxFactory.DiscardPattern(), 
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, 
                        SyntaxFactory.Literal("This is not an apple")))
            ]));
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "WhatFruit")
            .AddParameterListParameters(SyntaxFactory.Parameter(default, default, fruitType, fruit.Identifier, null))
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), body))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void ScoreToGrade()
    {
        var score = SyntaxFactory.IdentifierName("score");
        var body = score.SwitchExpression()
            .Case(SyntaxGenerator.OrPattern(SyntaxGenerator.Literal(10), SyntaxGenerator.Literal(9)), SyntaxGenerator.Literal("优"))
            .Case(SyntaxGenerator.OrPattern(SyntaxGenerator.Literal(8), SyntaxGenerator.Literal(7)), SyntaxGenerator.Literal("良"))
            .Case(SyntaxGenerator.Literal(6), SyntaxGenerator.Literal("中"))
            .Default(SyntaxGenerator.Literal("差"))
            .Build();
        // bool IntToBool(int value)
        var method = SyntaxGenerator.StringType.Method("ScoreToGrade", SyntaxGenerator.IntType.Parameter(score.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //string ScoreToGrade(int score) => score switch
    //{
    //    10 or 9 => "优",
    //    6 => "中",
    //    _ => "差"
    //};
}
