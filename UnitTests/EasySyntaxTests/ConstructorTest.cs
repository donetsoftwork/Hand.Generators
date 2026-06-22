using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class ConstructorTest
{
    [Fact]
    public void Constructor()
    {
        var type = SyntaxFactory.ClassDeclaration("UserId");
        var original = SyntaxFactory.IdentifierName("original");
        var constructor = type.Constructor(SyntaxGenerator.IntType.Parameter(original.Identifier))
            .ToBuilder()
            .AddPatter(SyntaxFactory.IdentifierName("_original").Assign(original))
            .End();
        var code = constructor.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Constructor0()
    {
        var constructor = SyntaxFactory.ConstructorDeclaration("UserId")
            .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("original")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
            .AddBodyStatements(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_original"), SyntaxFactory.IdentifierName("original"))));
        var code = constructor.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void WithInitializer()
    {
        var constructor = SyntaxFactory.ConstructorDeclaration("Discounter")
            .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("percent")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword))))
            .WithBody(SyntaxFactory.Block());
        var constructor2 = SyntaxFactory.ConstructorDeclaration("Discounter")
            .WithInitializer(SyntaxFactory.ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0.9)))))))
            .WithBody(SyntaxFactory.Block());
        var discounterType = SyntaxFactory.ClassDeclaration("Discounter")
            .AddMembers(constructor, constructor2);
        var code = discounterType.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void WithInitializer2()
    {
        var discounterType = SyntaxFactory.ClassDeclaration("Discounter");
        var constructor = discounterType.Constructor(SyntaxGenerator.DoubleType.Parameter("percent"))            
            .WithBody(SyntaxFactory.Block());
        var constructor2 = discounterType.Constructor()
            .WithInitializer(SyntaxGenerator.Literal(0.9))
            .WithBody(SyntaxFactory.Block());
        discounterType = discounterType
            .AddMembers(constructor, constructor2);
        var code = discounterType.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
}
