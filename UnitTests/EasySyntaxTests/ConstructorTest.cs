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
            .Add(SyntaxFactory.IdentifierName("_original").Assign(original))
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
}
