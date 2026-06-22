using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Hand.SyntaxGenerator;

namespace EasySyntaxTests;

public class ForEachTests
{
    [Fact]
    public void Count()
    {
        var list = SyntaxFactory.IdentifierName("list");
        var item = SyntaxFactory.IdentifierName("item");
        var count = SyntaxFactory.IdentifierName("count");
        var method = SyntaxGenerator.IntType.Method("Count", SyntaxGenerator.IntType.Array().Parameter(list.Identifier))
            .ToBuilder()
            .Declare(SyntaxGenerator.VarType.Variable(count.Identifier, SyntaxGenerator.Literal(0)))
            .ForEach(SyntaxGenerator.VarType, item.Identifier, list)
                .AddPatter(count.AddAssign(item))
            .End()
            .Return(count);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Count0()
    {
        var list = SyntaxFactory.IdentifierName("list");
        var item = SyntaxFactory.IdentifierName("item");
        var count = SyntaxFactory.IdentifierName("count");
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "Count")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(list.Identifier)
                    .WithType(SyntaxFactory.ArrayType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))))
            .AddBodyStatements(
                SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"), SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(count.Identifier)
                            .WithInitializer(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                                SyntaxFactory.Literal(0)))))),
                SyntaxFactory.ForEachStatement(
                    SyntaxFactory.IdentifierName("var"), 
                    item.Identifier, 
                    list, 
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(SyntaxKind.AddAssignmentExpression, count, item))),
                SyntaxFactory.ReturnStatement(count)
            );
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Count2()
    {
        var list = SyntaxFactory.IdentifierName("list");
        var item = SyntaxFactory.IdentifierName("item");
        var count = SyntaxFactory.IdentifierName("count");
        // int Count(int[] list)
        var method = IntType.Method("Count", IntType.Array().Parameter(list.Identifier))
            .ToBuilder()
            // var count = 0
            .Declare(VarType.Variable(count.Identifier, Literal(0)))
            // foreach(item in list)
            .ForEach(item.Identifier, list)
                //count += item
                .AddPatter(count.AddAssign(item))
            .End()
            .Return(count);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Count3()
    {
        var list = SyntaxFactory.IdentifierName("list");
        var item = SyntaxFactory.IdentifierName("item");
        var count = SyntaxFactory.IdentifierName("count");

        // foreach(item in list)
        var forEach = list.ForEach(item.Identifier)
            //count += item
            .AddPatter(count.AddAssign(item))
            .Build();
        var body = SyntaxGenerator.Scope()
            // var count = 0
            .Declare(VarType.Variable(count.Identifier, Literal(0)))
            .Add(forEach)
            // return count
            .Add(count.Return())
            .Block();

        // int Count(int[] list)
        var method = IntType.Method("Count", IntType.Array().Parameter(list.Identifier))
            .WithBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
}
