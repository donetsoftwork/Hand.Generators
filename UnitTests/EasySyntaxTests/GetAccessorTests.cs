using Hand;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class GetAccessorTests
{
    [Fact]
    public void GetAccessor()
    {
        var property = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Id"), SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        var getAccessor = property.GetAccessor();
        Assert.NotNull(getAccessor);
        var setAccessor = property.GetSetAccessor();
        Assert.NotNull(setAccessor);
    }
    [Fact]
    public void GetInitAccessor()
    {
        var property = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Id"), SyntaxKind.GetAccessorDeclaration, SyntaxKind.InitAccessorDeclaration);
        var initAccessor = property.GetInitAccessor();
        Assert.NotNull(initAccessor);
        var setAccessor = property.GetSetAccessor();
        Assert.Null(setAccessor);
    }
}
