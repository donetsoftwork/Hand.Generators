using Hand;
using Hand.Members;
using Hand.Naming;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NamingTests;

public class KindProviderTests
{
    [Fact]
    public void AddKind()
    {
        var provider = new KindContainer();
        provider.AddKind("_id", MemberKind.Field);
        Assert.True(provider.Contains("_id"));
    }
    [Fact]
    public void TryDeclareField()
    {
        var provider = new KindContainer();
        Assert.True(provider.TryDeclareField("_id"));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareField("_id"));
    }
    [Fact]
    public void TryDeclareProperty()
    {
        var provider = new KindContainer();
        Assert.True(provider.TryDeclareProperty("Id"));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareProperty("Id"));
    }
    [Fact]
    public void AddProperty()
    {
        var source = @"
public partial class UserDto
{
    public string? Id { get; set; }
}
public record User(int Id, string Name);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver();
        SyntaxTree syntaxTree = driver.Parse(source);
        var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(destType);
        var compilation = driver.Compile(syntaxTree);
        var sourceSymbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName("UserDto");
        Assert.NotNull(destSymbol);
        var nameProvider = KindContainer.Create(destSymbol);
        var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();

        var generator = SyntaxGenerator.Clone(destType);
        foreach (var sourceProperty in sourceProperties)
        {
            var propertyName = sourceProperty.Name;
            if (nameProvider.Contains(propertyName))
                continue;
            var property = sourceProperty.Type.ToSyntax().GetSetProperty(propertyName).Public();
            generator.AddProperty(property);
            nameProvider.AddKind(propertyName, MemberKind.PropertyDeclaration);
        }
        var code = generator.Build().NormalizeWhitespace().ToFullString();
        Assert.Contains("Name", code);
        Assert.DoesNotContain("Id", code);
    }
    [Fact]
    public void TryGetMemberKind()
    {
        var provider = new KindContainer();
        Assert.False(provider.TryGetMemberKind("Id", out var kind));
        Assert.Equal(MemberKind.Unknown, kind);
        provider.AddKind("Id", MemberKind.PropertyDeclaration);
        Assert.True(provider.TryGetMemberKind("Id", out kind));
        Assert.Equal(MemberKind.PropertyDeclaration, kind);
    }
    [Fact]
    public void Create()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("record User(int Id, string Name);");
        var symbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(symbol);
        var provider = KindContainer.Create(symbol);
        Assert.True(provider.Contains("Id"));
        Assert.True(provider.Contains("Name"));
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
        var provider = KindContainer.Inherit(symbol);
        Assert.True(provider.Contains("Name"));
        Assert.True(provider.Contains("Hello"));
        Assert.True(provider.Contains("Balance"));
        Assert.True(provider.Contains("Pay"));
    }
}
