using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class PropertyTests
{
    [Fact]
    public void PropertyId()
    {
        var property = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Id"), SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        var code = property.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Id"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code0);
    }
    [Fact]
    public void PropertyName()
    {
        var property = SyntaxGenerator.StringType.GetOnlyProperty(SyntaxFactory.Identifier("Name"), SyntaxFactory.IdentifierName("_name"));
        var code = property.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), SyntaxFactory.Identifier("Name"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .AddBodyStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("_name")))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code0);

    }
    [Fact]
    public void PropertyAge()
    {
        var getAge = SyntaxGenerator.PropertyGetDeclaration().ToBuilder().Return(SyntaxFactory.IdentifierName("_age"));
        var setAge = SyntaxGenerator.PropertySetDeclaration().ToBuilder().Add(SyntaxFactory.IdentifierName("_age").AssignValue()).End();
        var property = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Age"), getAge, setAge);
        var code = property.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Age"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .AddBodyStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("_age"))),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .AddBodyStatements(SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_age"), SyntaxFactory.IdentifierName("value"))))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code0);
    }
}
