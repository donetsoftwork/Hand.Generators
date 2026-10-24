using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests.Collections;

public class TypeComparerEqualTests
{
    [Fact]
    public void IdentifierEqual()
    {
        var type1 = SyntaxFactory.IdentifierName("User");
        var type2 = SyntaxFactory.IdentifierName("User");
        Assert.False(type1.Equals(type2));
        Assert.True(TypeComparer.IdentifierEqual(type1, type2));
    }
    [Theory]
    [InlineData(SyntaxKind.BoolKeyword)]
    [InlineData(SyntaxKind.ByteKeyword)]
    [InlineData(SyntaxKind.SByteKeyword)]
    [InlineData(SyntaxKind.ShortKeyword)]
    [InlineData(SyntaxKind.IntKeyword)]
    [InlineData(SyntaxKind.UIntKeyword)]
    [InlineData(SyntaxKind.LongKeyword)]
    [InlineData(SyntaxKind.ULongKeyword)]
    [InlineData(SyntaxKind.DoubleKeyword)]
    [InlineData(SyntaxKind.FloatKeyword)]
    [InlineData(SyntaxKind.DecimalKeyword)]
    [InlineData(SyntaxKind.StringKeyword)]
    [InlineData(SyntaxKind.CharKeyword)]
    [InlineData(SyntaxKind.VoidKeyword)]
    [InlineData(SyntaxKind.ObjectKeyword)]
    public void Predefined(SyntaxKind kind)
    {
        var type1 = SyntaxFactory.PredefinedType(SyntaxFactory.Token(kind));
        var type2 = SyntaxFactory.PredefinedType(SyntaxFactory.Token(kind));
        Assert.False(type1.Equals(type2));
        Assert.True(TypeComparer.PredefinedEqual(type1, type2));
    }
}
