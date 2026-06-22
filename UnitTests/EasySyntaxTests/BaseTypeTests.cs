using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class BaseTypeTests
{
    [Fact]
    public void AddBaseListTypes()
    {
        var vipType = SyntaxFactory.ClassDeclaration("Vip")
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("Customer")));
        var code = vipType.NormalizeWhitespace().ToFullString();
        Assert.Equal("class Vip : Customer\r\n{\r\n}", code);
    }
    [Fact]
    public void AddBaseTypes()
    {
        var vipType = SyntaxFactory.ClassDeclaration("Vip")
            .AddBaseTypes("Customer");
        var code = vipType.NormalizeWhitespace().ToFullString();
        Assert.Equal("class Vip : Customer\r\n{\r\n}", code);
    }
}
