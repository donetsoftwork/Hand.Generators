using Hand;
using Hand.Members;
using Hand.Naming;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.Common;
using static NamingTests.TypedProviderTests;

namespace NamingTests;

public class TypedProviderTests
{
    [Fact]
    public void AddKind()
    {
        var provider = new TypedProvider("User");
        provider.AddKind("_id", MemberKind.Field);
        Assert.True(provider.Contains("_id"));
        Assert.True(provider.Contains("User"));
    }
    [Fact]
    public void TryDeclareField()
    {
        var provider = new TypedProvider("User");
        Assert.True(provider.TryDeclareField("_id"));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareField("_id"));
        Assert.False(provider.TryDeclareField("User"));
    }
    [Fact]
    public void TryDeclareProperty()
    {
        var provider = new TypedProvider("User");
        Assert.True(provider.TryDeclareProperty("Id"));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareProperty("Id"));
        Assert.False(provider.TryDeclareProperty("User"));
    }
    [Fact]
    public void TryGetMemberKind()
    {
        var provider = new TypedProvider("User");
        Assert.False(provider.TryGetMemberKind("Id", out var kind));
        Assert.Equal(MemberKind.Unknown, kind);
        provider.AddKind("Id", MemberKind.PropertyDeclaration);
        Assert.True(provider.TryGetMemberKind("Id", out kind));
        Assert.Equal(MemberKind.PropertyDeclaration, kind);
        Assert.True(provider.TryGetMemberKind("User", out kind));
        Assert.Equal(MemberKind.Unknown, kind);
    }
    [Fact]
    public void AddField()
    {
        var source = @"
public partial class B;
public record A(string Name, string B);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver();
        SyntaxTree syntaxTree = driver.Parse(source);
        var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(destType);
        var compilation = driver.Compile(syntaxTree);
        var sourceSymbol = compilation.GetTypeByMetadataName("A");
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName("B");
        Assert.NotNull(destSymbol);
        var nameProvider = TypedProvider.Create(destSymbol);
        var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();

        var generator = SyntaxGenerator.Clone(destType);
        foreach (var sourceProperty in sourceProperties)
        {
            var propertyName = sourceProperty.Name;
            if (nameProvider.TryDeclareField(propertyName))
            {
                var field = sourceProperty.Type.ToSyntax()
                    .Field(propertyName, SyntaxGenerator.DefaultLiteral.SuppressNull())
                    .Public();
                generator.AddField(field);
            }
        }
        var code = generator.Build().NormalizeWhitespace().ToFullString();
        Assert.Contains("Name", code);
    }
    //partial class B
    //{
    //    public string Name = default!;
    //    public string B = default!;
    //}
    [Fact]
    public void Create()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("record User(int Id, string Name);");
        var symbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(symbol);
        var provider = TypedProvider.Create(symbol);
        Assert.True(provider.Contains("Id"));
        Assert.True(provider.Contains("Name"));
        Assert.True(provider.Contains("User"));
    }
    [Fact]
    public void Inherit()
    {
        var source = @"
    class Account : User
    {
        public decimal Balance { get; set; }
        public void Pay(Account other) { }
    }
    class User
    {
        public string Name { get; set;  }
        public void Hello(User other) { }
    }
";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(source);
        var symbol = compilation.GetTypeByMetadataName("Account");
        Assert.NotNull(symbol);
        var provider = TypedProvider.Inherit(symbol);
        Assert.True(provider.Contains("Name"));
        Assert.True(provider.Contains("Hello"));
        Assert.True(provider.Contains("Account"));
        Assert.True(provider.Contains("Balance"));
        Assert.True(provider.Contains("Pay"));
        Assert.False(provider.Contains("User"));
    }
    [Fact]
    public void New()
    {
        var source = @"
    public record Column(string Name);
    public abstract class Table(string name)
    {
        public string Name { get; } = name;
        public abstract Column[] Columns { get; }
    }
    public record User(int Id, string Name);
    public partial class UserTable() : Table(""Users"");
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver();
        SyntaxTree syntaxTree = driver.Parse(source);
        var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().LastOrDefault();
        Assert.NotNull(destType);
        var compilation = driver.Compile(syntaxTree);
        var sourceSymbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName("UserTable");
        Assert.NotNull(destSymbol);
        var columnType = SyntaxFactory.IdentifierName("Column");
        var nameProvider = TypedProvider.Inherit(destSymbol);
        var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();

        var generator = SyntaxGenerator.Clone(destType);
        var columnList = new List<ExpressionSyntax>();
        foreach (var sourceProperty in sourceProperties)
        {
            var columnName = sourceProperty.Name;
            if (nameProvider.TryDeclareField(columnName, out var isNew))
            {
                var column = SyntaxFactory.IdentifierName(columnName);
                var field = columnType
                    .Field(column.Identifier, SyntaxGenerator.New([SyntaxGenerator.Literal(columnName)]))
                    .Public();
                if (isNew)
                    field = field.New();
                generator.AddField(field);
                columnList.Add(column);
            }
        }
        var columns = columnType.Array()
            .Property("Columns", SyntaxGenerator.Collection([.. columnList]))
            .Public()
            .Override();
        generator.AddProperty(columns);

        var code = generator.Build().NormalizeWhitespace().ToFullString();
        Assert.Contains("new Column Name", code);

    }
    //public record Column(string Name);
    //public abstract class Table(string name)
    //{
    //    public string Name { get; } = name;
    //    public abstract Column[] Columns { get; }
    //}
    //public record User(int Id, string Name);
    //public partial class UserTable() : Table("Users");
    //partial class UserTable
    //{
    //    public Column Id = new("Id");
    //    public new Column Name = new("Name");
    //    public override Column[] Columns => [Id, Name];
    //}
}
