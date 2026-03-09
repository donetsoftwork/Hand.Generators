using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EasySyntaxTests;

public class ForTests
{
    [Fact]
    public void Total()
    {
        var i = SyntaxFactory.IdentifierName("i");
        var num = SyntaxFactory.IdentifierName("num");
        var count = SyntaxFactory.IdentifierName("count");
        var method = SyntaxGenerator.IntType.Method("Total", SyntaxGenerator.IntType.Parameter(num.Identifier))
            .ToBuilder()
            // int count = 0
            .Declare(SyntaxGenerator.IntType.Variable(count.Identifier, SyntaxGenerator.Literal(0)))
            // for(var i=0;i<num;i++)
            .For(i, num)
                // count+=i
                .Add(count.AddAssign(i))
            .End()
            // return count
            .Return(count);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Total0()
    {
        var i = SyntaxFactory.IdentifierName("i");
        var num = SyntaxFactory.IdentifierName("num");
        var count = SyntaxFactory.IdentifierName("count");
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "Total")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(num.Identifier)
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
            .AddBodyStatements(
                SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"), SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(count.Identifier)
                            .WithInitializer(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                SyntaxFactory.Literal(0)))))),
                SyntaxFactory.ForStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var")).AddVariables(
                            SyntaxFactory.VariableDeclarator(i.Identifier)
                                .WithInitializer(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                    SyntaxFactory.Literal(0)))),
                    default,
                    SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, i, num),
                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, i)),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.AddAssignmentExpression, count, i))),
                SyntaxFactory.ReturnStatement(count)
            );
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Sum()
    {
        var parameter = SyntaxGenerator.IntType.Array(2).Parameter("list");
        var list = parameter.ToIdentifierName();
        var i = SyntaxFactory.IdentifierName("i");
        var j = SyntaxFactory.IdentifierName("j");
        var getLength = list.Access("GetLength");
        var count = SyntaxGenerator.IntType.Variable("count", SyntaxGenerator.Literal(0));
        var method = SyntaxGenerator.IntType.Method("Count", parameter)
            .ToBuilder()
            // int count=0
            .Declare(count)
            // for(var i = 0;i<list.GetLength(0);i++)
            .For(i, getLength.Invocation([SyntaxGenerator.Literal(0)]))
                // for(var j = 0;j<list.GetLength(1);j++)
                .For(j, getLength.Invocation([SyntaxGenerator.Literal(1)]))
                    // count+=list[i,j]
                    .Add(count.ToIdentifierName().AddAssign(list.Element([i, j])))
                .End()
            .End()
            // return count
            .Return(count.ToIdentifierName());
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Tree()
    {
        // 创建变量声明：int i = 0;
        VariableDeclarationSyntax declaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int"))
            .AddVariables(SyntaxFactory.VariableDeclarator("i").WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))));

        // 创建初始化表达式：i < 10
        ExpressionSyntax initialization = SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, SyntaxFactory.IdentifierName("i"), SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(10)));
        //SyntaxFactory.IndexerMember
        // 创建增量表达式：i++
        ExpressionSyntax increment = SyntaxFactory.IdentifierName("i").PostIncrement();

        // 创建循环体：Console.WriteLine(i);
        StatementSyntax statement = SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("Console"), SyntaxFactory.IdentifierName("WriteLine")),
            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("i"))))
        ));

        // ForStatement(VariableDeclarationSyntax? declaration, SeparatedSyntaxList<ExpressionSyntax> initializers, ExpressionSyntax? condition, SeparatedSyntaxList<ExpressionSyntax> incrementors, StatementSyntax statement)
        // 创建for循环语句
        //ForStatementSyntax forStatement = SyntaxFactory.ForStatement(statement)
        //    .WithDeclaration(declaration)
        //    .WithCondition(initialization)
        //    .AddIncrementors(increment);
        ForStatementSyntax forStatement = SyntaxFactory.ForStatement(declaration, default, initialization, SyntaxFactory.SingletonSeparatedList(increment), statement);

        // 将for语句添加到根语句列表中
        BlockSyntax root = SyntaxFactory.Block(forStatement);
        var code = root.NormalizeWhitespace().ToFullString();
        
        Assert.NotEmpty(code);
    }
}
