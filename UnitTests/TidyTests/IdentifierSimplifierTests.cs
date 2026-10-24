using Hand;
using Hand.Collections;
using Hand.Tidy;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TidyTests;

public class IdentifierSimplifierTests
{
    [Fact]
    public void IdentifierEquals()
    {
        var container = SyntaxFactory.IdentifierName("User");
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User");
        Assert.True(simplifier.Simplify(ref name));
        Assert.True(TypeComparer.Equal(container, name));
        var type = simplifier.Simplify(name);
        Assert.True(TypeComparer.Equal(name, type));
    }
    [Fact]
    public void IdentifierEquals0()
    {
        var container = SyntaxFactory.IdentifierName("User");
        var simplifier = new IdentifierSimplifier(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User");
        Assert.True(simplifier.Simplify(ref name));
        Assert.True(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void IdentifierNot()
    {
        var container = SyntaxFactory.IdentifierName("User");
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxFactory.IdentifierName("City");
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void IdentifierNot0()
    {
        var container = SyntaxFactory.IdentifierName("User");
        var simplifier = new IdentifierSimplifier(container);
        NameSyntax name = SyntaxFactory.IdentifierName("City");
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void Identifier()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxFactory.IdentifierName("A");
        Assert.True(simplifier.Simplify(ref name));
        Assert.Equal("A", name.ToFullString());
    }
    [Fact]
    public void Identifier0()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = new IdentifierSimplifier(container);
        NameSyntax name = SyntaxFactory.IdentifierName("A");
        Assert.True(simplifier.Simplify(ref name));
        Assert.Equal("A", name.ToFullString());
    }
    [Fact]
    public void Qualify()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify("A");
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
    [Fact]
    public void Qualify0()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = new IdentifierSimplifier(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify("A");
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("User", name.ToFullString());
    }
    [Fact]
    public void Qualify2()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("B").Qualify(SyntaxFactory.IdentifierName("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("B.User", name.ToFullString());
    }
    [Fact]
    public void Qualify20()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = new IdentifierSimplifier(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("B").Qualify(SyntaxFactory.IdentifierName("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("B.User", name.ToFullString());
    }
    [Fact]
    public void Qualify3()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("C").Qualify(SyntaxFactory.IdentifierName("B").Qualify("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("B.C.User", name.ToFullString());
    }
    [Fact]
    public void Qualify30()
    {
        var container = SyntaxFactory.IdentifierName("A");
        var simplifier = new IdentifierSimplifier(container);
        NameSyntax name = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("C").Qualify(SyntaxFactory.IdentifierName("B").Qualify("A")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("B.C.User", name.ToFullString());
    }
}
