using Hand;
using Hand.Collections;
using Hand.Tidy;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TidyTests;

public class GenericSimplifierTests
{
    [Fact]
    public void GenericEquals()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        Assert.True(simplifier.Simplify(ref name));
        Assert.True(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericEquals0()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        NameSyntax name = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        Assert.True(simplifier.Simplify(ref name));
        Assert.True(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericNot()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericNot0()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        NameSyntax name = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericNot2()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("City"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericNot20()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        NameSyntax name = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("City"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericNot3()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"), SyntaxFactory.IdentifierName("User"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void GenericNot30()
    {
        var container = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        NameSyntax name = SyntaxGenerator.Generic("DAL", SyntaxFactory.IdentifierName("User"), SyntaxFactory.IdentifierName("User"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.False(TypeComparer.Equal(container, name));
    }
    [Fact]
    public void Generic()
    {
        var container = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        Assert.True(simplifier.Simplify(ref name));
        Assert.Equal("BLL<User>", name.ToFullString());
    }
    [Fact]
    public void Generic0()
    {
        var container = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        NameSyntax name = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        Assert.True(simplifier.Simplify(ref name));
        Assert.Equal("BLL<User>", name.ToFullString());
    }
    [Fact]
    public void Generic2()
    {
        var container = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        NameSyntax name = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("City"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("BLL<City>", name.ToFullString());
    }
    [Fact]
    public void Generic20()
    {
        var container = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        NameSyntax name = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("City"));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("BLL<City>", name.ToFullString());
    }
    [Fact]
    public void Qualify()
    {
        // BLL<User>
        var container = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        // BLL<User>.Query
        NameSyntax name = SyntaxFactory.IdentifierName("Query").Qualify(SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("Query", name.ToFullString());
    }
    [Fact]
    public void Qualify0()
    {
        // BLL<User>
        var container = SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        // BLL<User>.Query
        NameSyntax name = SyntaxFactory.IdentifierName("Query").Qualify(SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("User")));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("Query", name.ToFullString());
    }
    [Fact]
    public void Qualify2()
    {
        // DataSet<User>
        var container = SyntaxGenerator.Generic("DataSet", SyntaxFactory.IdentifierName("User"));
        var simplifier = TypeSimplifier.Create(container);
        // DataSet<User>.BLL<UserDTO>.Query
        NameSyntax name = SyntaxFactory.IdentifierName("Query").Qualify(SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("UserDTO")).Qualify(container));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("BLL<UserDTO>.Query", name.ToFullString());
    }
    [Fact]
    public void Qualify20()
    {
        // DataSet<User>
        var container = SyntaxGenerator.Generic("DataSet", SyntaxFactory.IdentifierName("User"));
        var simplifier = new GenericSimplifier(container);
        // DataSet<User>.BLL<UserDTO>.Query
        NameSyntax name = SyntaxFactory.IdentifierName("Query").Qualify(SyntaxGenerator.Generic("BLL", SyntaxFactory.IdentifierName("UserDTO")).Qualify(container));
        Assert.False(simplifier.Simplify(ref name));
        Assert.Equal("BLL<UserDTO>.Query", name.ToFullString());
    }
}
