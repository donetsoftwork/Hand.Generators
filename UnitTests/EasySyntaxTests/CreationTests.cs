using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class CreationTests
{
    [Fact]
    public void TypeCreation()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var creation = type.New();
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new User()", code);
    }
    [Fact]
    public void TypeArgumentCreation()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var creation = type.New([SyntaxGenerator.Literal(1)]);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new User(1)", code);
    }
    [Fact]
    public void TypeInitializerCreation()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var userId = SyntaxFactory.IdentifierName("UserId").Assign(SyntaxGenerator.Literal(1));
        var creation = type.New([userId]);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("new User()", code);
        Assert.Contains("UserId = ", code);
    }
    [Fact]
    public void TypeInitializerCreation2()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var userName = SyntaxFactory.IdentifierName("UserName").Assign(SyntaxGenerator.Literal("Jxj"));
        var creation = type.New([userName]);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("new User()", code);
        Assert.Contains("UserName = ", code);
    }
    [Fact]
    public void TypeArgumentsAndInitializerCreation0()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var userId = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(1));
        var userName = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.IdentifierName("UserName"),
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal("Jxj")));

        var creation = SyntaxFactory.ObjectCreationExpression(type)
            .AddArgumentListArguments(SyntaxFactory.Argument(userId))
            .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, 
                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(userName)));
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("new User(1)", code);
        Assert.Contains("UserName = ", code);
    }
    [Fact]
    public void TypeArgumentsAndInitializerCreation()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var userName= SyntaxFactory.IdentifierName("UserName").Assign(SyntaxGenerator.Literal("Jxj"));
        var creation = type.New([SyntaxGenerator.Literal(1)], [userName]);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("new User(1)", code);
        Assert.Contains("UserName = ", code);
    }
    [Fact]
    public void ListCreation()
    {
        var type = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        var creation = type.New();
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new List<int>()", code);
    }
    [Fact]
    public void ListArgumentCreation()
    {
        var type = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        var creation = type.New([SyntaxGenerator.Literal(10)]);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new List<int>(10)", code);
    }
    [Fact]
    public void ImplicitObjectCreation()
    {
        var expression = SyntaxFactory.ParseExpression("new()");
        Assert.NotNull(expression);
        var creation = SyntaxFactory.ImplicitObjectCreationExpression();
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new()", code);
    }
    [Fact]
    public void ObjectCreationExpression()
    {
        var creation = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("User"))
            .AddArgumentListArguments(SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(10))));
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new User(10)", code);
    }
    [Fact]
    public void CreationWithInitializer()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var userName = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.IdentifierName("UserName"),
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal("Jxj")));
        var creation = SyntaxFactory.ObjectCreationExpression(
            type, 
            SyntaxFactory.ArgumentList(),
            SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(userName)));
        var variable = type.Variable("user", creation);
        Assert.NotNull(variable);
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("User user = new User()", code);
        Assert.Contains("UserName = ", code);
    }
    [Fact]
    public void ImplicitWithInitializer()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var userName = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.IdentifierName("UserName"),
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal("Jxj")));
        var creation = SyntaxFactory.ImplicitObjectCreationExpression(
            SyntaxFactory.ArgumentList(),
            SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(userName)));
        var variable = type.Variable("user", creation);
        Assert.NotNull(variable);
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("User user = new()", code);
        Assert.Contains("UserName = ", code);
    }
    [Fact]
    public void ImplicitObjectArgumentCreation()
    {
        var creation = SyntaxFactory.ImplicitObjectCreationExpression()
            .AddArgumentListArguments(SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                SyntaxFactory.Literal(10))));
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new(10)", code);
    }
    [Fact]
    public void ArrayCreation0()
    {
        var type = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
        var one = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
        var two = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2));
        var creation = SyntaxFactory.ArrayCreationExpression(
            SyntaxFactory.Token(SyntaxKind.NewKeyword),
            SyntaxFactory.ArrayType(
                type, 
                SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression())))), 
            SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>([one, two])));
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("new int[]", code);
    }
    [Fact]
    public void ArrayCreation()
    {
        var type = SyntaxGenerator.IntType;
        var one = SyntaxGenerator.Literal(1);
        var two = SyntaxGenerator.Literal(2);
        var creation = type.NewArray(one, two);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("new int[]", code);
    }
    [Fact]
    public void Collection0()
    {
        var one = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
        var two = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2));
        var creation = SyntaxFactory.CollectionExpression(
            SyntaxFactory.SeparatedList<CollectionElementSyntax>([
                SyntaxFactory.ExpressionElement(one), 
                SyntaxFactory.ExpressionElement(two)]));
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("[1, 2]", code);
    }
    [Fact]
    public void Collection()
    {
        //var collection0 = SyntaxFactory.CollectionExpression(SyntaxFactory.SeparatedList<CollectionElementSyntax>([SyntaxFactory.ExpressionElement(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))), SyntaxFactory.ExpressionElement(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2)))]));
        var one = SyntaxGenerator.Literal(1);
        var two = SyntaxGenerator.Literal(2);
        var creation = SyntaxGenerator.Collection(one, two);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("[1, 2]", code);
    }
    [Fact]
    public void CollectionExpression()
    {
        var statement = SyntaxFactory.ParseStatement("int[] collection = [];");
        Assert.NotNull(statement);
        var creation = SyntaxFactory.CollectionExpression();
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("[]", code);
    }
    [Fact]
    public void Tuple()
    {
        var tuple0 = SyntaxFactory.TupleExpression(SyntaxFactory.SeparatedList([SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))), SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2)))]));
        var code0 = tuple0.NormalizeWhitespace().ToFullString();
        Assert.Equal("(1, 2)", code0);
        var one = SyntaxGenerator.Literal(1);
        var two = SyntaxGenerator.Literal(2);
        var creation = SyntaxGenerator.Tuple(one, two);
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("(1, 2)", code);
    }
}
