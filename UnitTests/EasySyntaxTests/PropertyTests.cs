using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class PropertyTests
{
    [Fact]
    public void PropertyId()
    {
        var property = SyntaxGenerator.IntType.GetSetProperty("Id");
        var code = property.NormalizeWhitespace().ToFullString();

        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Id"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void PropertyId2()
    {
        var property = SyntaxGenerator.IntType.GetInitProperty("Id");
        var code = property.NormalizeWhitespace().ToFullString();

        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Id"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void PropertyName()
    {
        var property = SyntaxGenerator.StringType.Property("Name", SyntaxFactory.IdentifierName("_name"));
        var code = property.NormalizeWhitespace().ToFullString();

        var property0 = SyntaxFactory.PropertyDeclaration(default, default, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), default, SyntaxFactory.Identifier("Name"), default, SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), SyntaxFactory.IdentifierName("_name")), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void PropertyAge()
    {
        var property = SyntaxGenerator.IntType.GetSetProperty("Age", SyntaxFactory.IdentifierName("_age"));
        var code = property.NormalizeWhitespace().ToFullString();

        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Age"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), SyntaxFactory.IdentifierName("_age")))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_age"), SyntaxFactory.IdentifierName("value"))))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void PropertySex()
    {
        var property = SyntaxGenerator.IntType.GetInitProperty("Sex", SyntaxFactory.IdentifierName("_sex"));
        var code = property.NormalizeWhitespace().ToFullString();

        var property0 = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Sex"))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), SyntaxFactory.IdentifierName("_sex")))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_sex"), SyntaxFactory.IdentifierName("value"))))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void PropertySex2()
    {
        var property = SyntaxGenerator.IntType.SetOnlyProperty("Sex", SyntaxFactory.IdentifierName("_sex"));
        var code = property.NormalizeWhitespace().ToFullString();

        var property0 = SyntaxFactory.PropertyDeclaration(default, default, SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), default, SyntaxFactory.Identifier("Sex"), default, SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_sex"), SyntaxFactory.IdentifierName("value"))), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        var code0 = property0.NormalizeWhitespace().ToFullString();
        Assert.Equal(code0, code);
    }
}

//public class User(string name)
//{
//    public int Id { get; set; }
//    private readonly string _name = name;
//    public string Name => _name;
//    private int _age = 1;
//    public int Age { get => _age; set => _age = value; }
//    private int _sex = 1;
//    public int Sex { get => _sex; init => _sex = value; }
//}