using Hand;
using Hand.Tidy;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TidyTests;

public class IdentifierQualifiedSimplifierTests
{
    [Fact]
    public void QualifiedEquals()
    {
        var simplifier = TypeSimplifier.Create(SyntaxFactory.IdentifierName("A"), SyntaxFactory.IdentifierName("User"));
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify("A");
        Assert.True(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
    [Fact]
    public void QualifiedEquals0()
    {
        var simplifier = new IdentifierQualifiedSimplifier(SyntaxFactory.IdentifierName("A"), [SyntaxFactory.IdentifierName("User")]);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify("A");
        Assert.True(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }

    [Fact]
    public void QualifiedPrefix2()
    {
        var simplifier = TypeSimplifier.Create(SyntaxFactory.IdentifierName("A"), SyntaxFactory.IdentifierName("B"));
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("B").Qualify(SyntaxFactory.IdentifierName("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
    [Fact]
    public void QualifiedPrefix20()
    {
        var simplifier = new IdentifierQualifiedSimplifier(SyntaxFactory.IdentifierName("A"), [SyntaxFactory.IdentifierName("B")]);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("B").Qualify(SyntaxFactory.IdentifierName("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
    [Fact]
    public void QualifiedPrefix3()
    {
        var simplifier = TypeSimplifier.Create(SyntaxFactory.IdentifierName("B").Qualify(SyntaxFactory.IdentifierName("A")), SyntaxFactory.IdentifierName("C"));
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("C").Qualify(SyntaxFactory.IdentifierName("B").Qualify("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
    [Fact]
    public void QualifiedPrefix30()
    {
        var simplifier = new IdentifierQualifiedSimplifier(SyntaxFactory.IdentifierName("A"), [SyntaxFactory.IdentifierName("B"), SyntaxFactory.IdentifierName("C")]);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("C").Qualify(SyntaxFactory.IdentifierName("B").Qualify("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
}
