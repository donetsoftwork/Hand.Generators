using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class SyntaxFactoryTests
{
    [Fact]
    public void Identifier()
    {
        var name = SyntaxFactory.Identifier("Name");
        Assert.Equal("Name", name.ValueText);
        var ns = SyntaxFactory.Identifier("System.Text");
        Assert.Equal("System.Text", ns.ValueText);

        //SyntaxNode node = SyntaxFactory.ParseName("System.Text");
    }
}
