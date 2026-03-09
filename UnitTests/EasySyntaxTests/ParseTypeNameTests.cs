using Hand;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class ParseTypeNameTests
{
    [Fact]
    public void PredefinedType()
    {
        var type = SyntaxFactory.ParseTypeName("int");
        if (type is not PredefinedTypeSyntax predefinedType)
        {
            Assert.Fail();
            return;
        }
        var predefinedType0 = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
        Assert.Equal(predefinedType0.Keyword.Kind(), predefinedType.Keyword.Kind());
    }
    [Fact]
    public void IdentifierName()
    {
        var type = SyntaxFactory.ParseTypeName("User");
        if (type is not IdentifierNameSyntax identifierName)
        {
            Assert.Fail();
            return;
        }
        var identifierName0 = SyntaxFactory.IdentifierName("User");
        Assert.Equal(identifierName0.ToFullString(), identifierName.ToFullString());
    }
    [Fact]
    public void QualifiedName()
    {
        var type = SyntaxFactory.ParseTypeName("Models.User");
        if (type is not QualifiedNameSyntax qualifiedName)
        {
            Assert.Fail();
            return;
        }
        var qualifiedName0 = SyntaxFactory.IdentifierName("User").Qualified("Models");
        Assert.Equal(qualifiedName0.ToFullString(), qualifiedName.ToFullString());
        var identifierName = SyntaxFactory.IdentifierName("Models.User");
        Assert.Equal(qualifiedName0.ToFullString(), identifierName.ToFullString());
    }
    [Fact]
    public void GenericName()
    {
        var type = SyntaxFactory.ParseTypeName("List<int>");
        if (type is not GenericNameSyntax genericName)
        {
            Assert.Fail();
            return;
        }
        var genericName0 = SyntaxFactory.GenericName("List")
            .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        Assert.Equal(genericName0.ToFullString(), genericName.ToFullString());
    }
    [Fact]
    public void QualifiedGenericName()
    {
        var type = SyntaxFactory.ParseTypeName("System.Collections.Generic.List<int>");
        if (type is not QualifiedNameSyntax qualifiedName)
        {
            Assert.Fail();
            return;
        }
        var genericName0 = SyntaxFactory.GenericName("List")
            .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        var qualifiedName0 = genericName0.Qualified("System.Collections.Generic");
        Assert.Equal(qualifiedName0.ToFullString(), qualifiedName.ToFullString());
        var genericName2 = SyntaxFactory.GenericName("System.Collections.Generic.List")
            .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        Assert.Equal(qualifiedName0.ToFullString(), genericName2.ToFullString());
    }
    [Fact]
    public void ArrayType()
    {
        var type = SyntaxFactory.ParseTypeName("int[]");
        if (type is not ArrayTypeSyntax arrayType)
        {
            Assert.Fail();
            return;
        }
        var arrayType0 = SyntaxFactory.ArrayType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
            .AddRankSpecifiers(SyntaxFactory.ArrayRankSpecifier());
        Assert.Equal(arrayType0.ToFullString(), arrayType.ToFullString());
    }
}