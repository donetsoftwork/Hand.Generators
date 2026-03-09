using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class SyntaxTreeTests
{
    [Fact]
    public void Field()
    {
        var original = SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("_id"))
                )
            ))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
        Assert.NotNull(original);
        var field = SyntaxGenerator.IntType.Field("_id")
             .Private();
        Assert.NotNull(field);
        var syntaxTree = field.SyntaxTree;
        Assert.NotNull(syntaxTree);
        Assert.Equal(field, syntaxTree.GetRoot());
    }
    [Fact]
    public void Property()
    {
        var field = SyntaxGenerator.IntType.Field("_id")
             .Private();
        var propertyGet = SyntaxGenerator.PropertyGetDeclaration()
            .ToBuilder()
            .Return(field.ToIdentifierName());
        var property = SyntaxGenerator.IntType.Property("Id", propertyGet)
            .Public();
        Assert.NotNull(property);
        var syntaxTree = property.SyntaxTree;
        Assert.NotNull(syntaxTree);
        Assert.Equal(property, syntaxTree.GetRoot());

    }
    [Fact] 
    public void Parameter()
    {
        var original = SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)));
        Assert.NotNull(original);

        var parameter = SyntaxGenerator.StringType.Field("name");
        Assert.NotNull(parameter);
        var syntaxTree = parameter.SyntaxTree;
        Assert.NotNull(syntaxTree);
        Assert.Equal(parameter, syntaxTree.GetRoot());
        var code = parameter.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Method()
    {
        var a = SyntaxGenerator.IntType.Parameter("a");
        var b = SyntaxGenerator.IntType.Parameter("b", SyntaxGenerator.Literal(1));
        var c = SyntaxGenerator.IntType.Variable("c", a.ToIdentifierName().Add(b.ToIdentifierName()));
        var method = SyntaxGenerator.IntType.Method("Add", a, b)
            .Public()
            .Static()
            .ToBuilder()
            .Declare(c)
            .Return(c.ToIdentifierName());
        Assert.NotNull(method);
        var syntaxTree = method.SyntaxTree;
        Assert.NotNull(syntaxTree);
        Assert.Equal(method, syntaxTree.GetRoot());
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Class()
    {
        var parameter = SyntaxGenerator.IntType.Parameter("id");
        var field = SyntaxGenerator.IntType.Field("_id", parameter.ToIdentifierName())
             .Private()
             .ReadOnly();
        var propertyGet = SyntaxGenerator.PropertyGetDeclaration()
            .ToBuilder()
            .Return(field.ToIdentifierName());
        var property = SyntaxGenerator.IntType.Property("Id", propertyGet)
            .Public();
        var type = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddParameterListParameters(parameter)
            .AddMembers(field, property);
        Assert.NotNull(type);
        var syntaxTree = type.SyntaxTree;
        Assert.NotNull(syntaxTree);
        Assert.Equal(type, syntaxTree.GetRoot());
        var code0 = type.ToFullString();
        Assert.NotEmpty(code0);
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Namespace()
    {
        var field = SyntaxGenerator.IntType.Field("_id")
             .Private();
        var property = SyntaxGenerator.IntType.GetOnlyProperty("Id", field.ToIdentifierName())
            .Public();
        var type = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(field, property);
        var ns = SyntaxGenerator.NamespaceDeclaration("Models")
            .AddMembers(type);
        Assert.NotNull(ns);
        var syntaxTree = ns.SyntaxTree;
        Assert.NotNull(syntaxTree);
        Assert.Equal(ns, syntaxTree.GetRoot());
    }
}
