using Hand;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class GenericNameTests
{
    [Fact]
    public void ListInt()
    {
        var listIntType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        var code = listIntType.ToFullString();
        Assert.Equal("List<int>", code);
    }
    [Fact]
    public void ListOmitted()
    {
        var listIntType = SyntaxGenerator.Generic("List", SyntaxFactory.OmittedTypeArgument());
        var code = listIntType.ToFullString();
        Assert.Equal("List<>", code);
    }
    [Fact]
    public void IList()
    {
        //SyntaxFactory.Token(SyntaxKind.I)
        //SyntaxFactory.Identifier(SyntaxFactory.)
    }
}
