using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class StylesTests
{
    [Fact]
    public void Nomal()
    {
        var Id = SyntaxGenerator.IntType.GetSetProperty("Id")
            .Public();
        var Name = SyntaxGenerator.StringType.GetSetProperty("Name")
            .Public();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void NomalWithInitializer()
    {
        var Id = SyntaxGenerator.IntType.GetSetProperty("Id")            
            .Public()
            .WithInitializer(SyntaxGenerator.Literal(1))
            .WithSemicolonToken();
        var Name = SyntaxGenerator.StringType.GetSetProperty("Name")            
            .Public()
            .WithInitializer(SyntaxGenerator.Literal(""))
            .WithSemicolonToken();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Init()
    {
        var Id = SyntaxGenerator.IntType.GetInitProperty("Id")
            .Public();
        var Name = SyntaxGenerator.StringType.GetInitProperty("Name")
            .Public();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void InitWithInitializer()
    {
        var Id = SyntaxGenerator.IntType.GetInitProperty("Id")
            .Public()
            .WithInitializer(SyntaxGenerator.Literal(1))
            .WithSemicolonToken();
        var Name = SyntaxGenerator.StringType.GetInitProperty("Name")
            .Public()
            .WithInitializer(SyntaxGenerator.Literal(""))
            .WithSemicolonToken();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Field()
    {
        var _id = SyntaxGenerator.IntType.Field("_id")
            .Private();
        var Id = SyntaxGenerator.IntType.GetSetProperty("Id", _id.ToIdentifierName())
            .Public();
        var _name = SyntaxGenerator.StringType.Field("_name")
            .Private();
        var Name = SyntaxGenerator.StringType.GetSetProperty("Name", _name.ToIdentifierName())
            .Public();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(_id, _name, Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void FieldWithInitializer()
    {
        var _id = SyntaxGenerator.IntType.Field("_id", SyntaxGenerator.Literal(1))
            .Private();
        var Id = SyntaxGenerator.IntType.GetSetProperty("Id", _id.ToIdentifierName())
            .Public();
        var _name = SyntaxGenerator.StringType.Field("_name", SyntaxGenerator.Literal(""))
            .Private();
        var Name = SyntaxGenerator.StringType.GetSetProperty("Name", _name.ToIdentifierName())
            .Public();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddMembers(_id, _name, Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Constructor()
    {
        var id = SyntaxFactory.IdentifierName("id");
        var Id = SyntaxFactory.IdentifierName("Id");
        var IdProperty = SyntaxGenerator.IntType.GetOnlyProperty(Id.Identifier)
            .Public();
        var name = SyntaxFactory.IdentifierName("name");
        var Name = SyntaxFactory.IdentifierName("Name");
        var NameProperty = SyntaxGenerator.StringType.GetOnlyProperty(Name.Identifier)
            .Public();
        var User = SyntaxFactory.Identifier("User");
        var constructor = SyntaxGenerator.ConstructorDeclaration(
                User,
                SyntaxGenerator.IntType.Parameter(id.Identifier),
                SyntaxGenerator.StringType.Parameter(name.Identifier))
            .Public()
            .ToBuilder()
                .AddPatter(Id.Assign(id))
                .AddPatter(Name.Assign(name))
            .End();
        var type = SyntaxFactory.ClassDeclaration("User")
             .Public()
             .AddMembers(constructor, IdProperty, NameProperty);

        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void ConstructorWithField()
    {
        var id = SyntaxFactory.IdentifierName("id");
        var _id = SyntaxFactory.IdentifierName("_id");
        var _idField = SyntaxGenerator.IntType.Field(_id.Identifier)
            .Private()
            .ReadOnly();
        var Id = SyntaxFactory.IdentifierName("Id");
        var IdProperty = SyntaxGenerator.IntType.Property(Id.Identifier, _id)
            .Public();
        var name = SyntaxFactory.IdentifierName("name");
        var _name = SyntaxFactory.IdentifierName("_name");
        var _nameField = SyntaxGenerator.StringType.Field(_name.Identifier)
            .Private()
            .ReadOnly();
        var Name = SyntaxFactory.IdentifierName("Name");
        var NameProperty = SyntaxGenerator.StringType.Property(Name.Identifier, _name)
            .Public();
        var User = SyntaxFactory.Identifier("User");
        var constructor = SyntaxGenerator.ConstructorDeclaration(
                User,
                SyntaxGenerator.IntType.Parameter(id.Identifier),
                SyntaxGenerator.StringType.Parameter(name.Identifier))
            .Public()
            .ToBuilder()
                .AddPatter(_id.Assign(id))
                .AddPatter(_name.Assign(name))
            .End();
        var type = SyntaxFactory.ClassDeclaration("User")
             .Public()
             .AddMembers(constructor, _idField, _nameField, IdProperty, NameProperty);

        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Parameter()
    {
        var id = SyntaxFactory.IdentifierName("id");
        var Id = SyntaxGenerator.IntType.GetOnlyProperty("Id")            
            .Public()
            .WithInitializer(id)
            .WithSemicolonToken();
        var name =SyntaxFactory.IdentifierName("name");
        var Name = SyntaxGenerator.StringType.GetOnlyProperty("Name")
            .Public()
            .WithInitializer(name)            
            .WithSemicolonToken();
        var type = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddParameterListParameters(
                SyntaxGenerator.IntType.Parameter(id.Identifier),
                SyntaxGenerator.StringType.Parameter(name.Identifier))
            .AddMembers(Id, Name);
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void ParameterWithField()
    {
        var id = SyntaxFactory.IdentifierName("id");
        var _id = SyntaxGenerator.IntType.Field("_id", id)
            .Private()
            .ReadOnly();
        var Id = SyntaxGenerator.IntType.Property("Id", _id.ToIdentifierName())
            .Public();
        var name = SyntaxFactory.IdentifierName("name");
        var _name = SyntaxGenerator.StringType.Field("_name", name)
            .Private()
            .ReadOnly();
        var Name = SyntaxGenerator.StringType.Property("Name", _name.ToIdentifierName())
            .Public();
        var User = SyntaxFactory.ClassDeclaration("User")
            .Public()
            .AddParameterListParameters(
                SyntaxGenerator.IntType.Parameter(id.Identifier),
                SyntaxGenerator.StringType.Parameter(name.Identifier))
            .AddMembers(_id, _name, Id, Name);
        var code = User.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Record()
    {
        var type = SyntaxGenerator.RecordDeclaration("User")
            .Public()
            .AddParameterListParameters(
                SyntaxGenerator.IntType.Parameter("Id"),
                SyntaxGenerator.StringType.Parameter("Name"))
            .WithSemicolonToken();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void RecordWithInitializer()
    {
        var type = SyntaxGenerator.RecordDeclaration("User")
            .Public()
            .AddParameterListParameters(
                SyntaxGenerator.IntType.Parameter("Id", SyntaxGenerator.Literal(1)),
                SyntaxGenerator.StringType.Parameter("Name", SyntaxGenerator.Literal("")))
            .WithSemicolonToken();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void RecordStruct()
    {
        var type = SyntaxGenerator.RecordStructDeclaration("User")
            .Public()
            .AddParameterListParameters(
                SyntaxGenerator.IntType.Parameter("Id"),
                SyntaxGenerator.StringType.Parameter("Name"))
            .WithSemicolonToken();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void RecordStructWithInitializer()
    {
        var type = SyntaxGenerator.RecordStructDeclaration("User")
            .Public()
            .AddParameterListParameters(
                SyntaxGenerator.IntType.Parameter("Id", SyntaxGenerator.Literal(1)),
                SyntaxGenerator.StringType.Parameter("Name", SyntaxGenerator.Literal("")))
            .WithSemicolonToken();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //public class User
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
    //public class User
    //{
    //    public int Id { get; set; } = 1;
    //    public string Name { get; set; } = "";
    //}
    //public class User
    //{
    //    public int Id { get; init; }
    //    public string Name { get; init; }
    //}
    //public class User
    //{
    //    public int Id { get; init; } = 1;
    //    public string Name { get; init; } = "";
    //}
    //public class User
    //{
    //    private int _id;
    //    private string _name;
    //    public int Id { get => _id; set => _id = value; }
    //    public string Name { get => _name; set => _name = value; }
    //}
    //public class User
    //{
    //    private int _id = 1;
    //    private string _name = "";
    //    public int Id { get => _id; set => _id = value; }
    //    public string Name { get => _name; set => _name = value; }
    //}
    //public class User
    //{
    //    public User(int id, string name)
    //    {
    //        Id = id;
    //        Name = name;
    //    }

    //    public int Id { get; }
    //    public string Name { get; }
    //}
    //public class User
    //{
    //    public User(int id, string name)
    //    {
    //        _id = id;
    //        _name = name;
    //    }

    //    private readonly int _id;
    //    private readonly string _name;
    //    public int Id => _id;
    //    public string Name => _name;
    //}
    //public class User(int id, string name)
    //{
    //    public int Id { get; } = id;
    //    public string Name { get; } = name;
    //}
    //public class User(int id, string name)
    //{
    //    private readonly int _id = id;
    //    private readonly string _name = name;
    //    public int Id => _id;
    //    public string Name => _name;
    //}
    //public record User(int Id, string Name);
    //public record User(int Id = 1, string Name = "");
    //public record struct User(int Id, string Name);
    //public record struct User(int Id = 1, string Name = "");
}
