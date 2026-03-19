using Hand;
using Microsoft.CodeAnalysis;
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
    public void IdentifierName2()
    {
        var type = SyntaxFactory.IdentifierName("User");
        var user = SyntaxFactory.IdentifierName("user");
        // User user = new()
        var variable = type.Variable(user.Identifier, SyntaxFactory.ImplicitObjectCreationExpression());
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.Equal("User user = new()", code);
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
        var genericName0 = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        Assert.Equal(genericName0.ToFullString(), genericName.ToFullString());

        // List<int>
        var listType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        Assert.NotNull(listType);
        // GetFieldValue<int>
        var method = SyntaxGenerator.Generic("GetFieldValue", SyntaxGenerator.IntType);
        Assert.NotNull(method);
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
        var qualifiedName0 = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualified("System.Collections.Generic");
        Assert.Equal(qualifiedName0.ToFullString(), qualifiedName.ToFullString());
        var genericName2 = SyntaxGenerator.Generic("System.Collections.Generic.List", SyntaxGenerator.IntType);
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