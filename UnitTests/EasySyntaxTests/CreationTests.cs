using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class CreationTests
{
    [Fact]
    public void ObjectCreation()
    {
        var type = SyntaxFactory.IdentifierName("Object");
        var creation = type.New();
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new Object()", code);
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
    public void ImplicitObjectArgumentCreation()
    {
        var creation = SyntaxFactory.ImplicitObjectCreationExpression()
            .AddArgumentListArguments(SyntaxFactory.Argument(SyntaxGenerator.Literal(10)));
        var code = creation.NormalizeWhitespace().ToFullString();
        Assert.Equal("new(10)", code);
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
}
