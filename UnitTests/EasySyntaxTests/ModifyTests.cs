using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class ModifyTests
{
    [Fact]
    public void Public()
    {
        var original = SyntaxFactory.PropertyDeclaration(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            SyntaxFactory.Identifier("Id"))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            );
        Assert.NotNull(original);
        var code0 = original.ToFullString();
        Assert.NotEmpty(code0);
        var property = SyntaxGenerator.IntType.Property("Id",
            SyntaxKind.GetAccessorDeclaration,
            SyntaxKind.InitAccessorDeclaration)
            .Public();
        Assert.NotNull(property);
        var code = property.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void ReadOnly()
    {
        var original = SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("_id"))
                )
            ))
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
        Assert.NotNull(original);
        var field = SyntaxGenerator.IntType.Field("_id")
             .Private()
             .ReadOnly();
        Assert.NotNull(field);
        var code = field.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Ref()
    {
        var original = SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.RefKeyword));
        Assert.NotNull(original);

        var parameter = SyntaxGenerator.StringType.Parameter("name")
            .Ref();
        Assert.NotNull(parameter);
        var code = parameter.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Virtual()
    {
        var original = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "CreateId")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
            .WithBody(
                SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
                    SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, SyntaxFactory.IdentifierName("_seed"))))
            );
        var code0 = original.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code0);
        var method = SyntaxGenerator.IntType.Method("CreateId")
            .Virtual()
            .ToBuilder()
            .Return(SyntaxFactory.IdentifierName("_seed").PostIncrement());
        Assert.NotNull(method);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Partial()
    {
        var original = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "CreateId")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        var code0 = original.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code0);

        var method = SyntaxGenerator.IntType.Method("CreateId")
            .Partial()
            .WithSemicolonToken();
        Assert.NotNull(method);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
}
