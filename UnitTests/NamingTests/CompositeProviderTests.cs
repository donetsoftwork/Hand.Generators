using Hand;
using Hand.Members;
using Hand.Naming;
using Hand.Reflection;
using Hand.Words;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NamingTests;

public class CompositeProviderTests
{
    [Fact]
    public void AddKind()
    {
        var provider = new CompositeProvider();
        provider.AddKind("_id", MemberKind.Field);
        Assert.True(provider.Contains("_id"));
    }
    [Fact]
    public void TryDeclareField()
    {
        var provider = new CompositeProvider();
        Assert.True(provider.TryDeclareField("_id"));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareField("_id"));
    }
    [Fact]
    public void TryDeclareProperty()
    {
        var provider = new CompositeProvider();
        Assert.True(provider.TryDeclareProperty("Id"));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareProperty("Id"));
    }
    [Fact]
    public void TryGetMemberKind()
    {
        var provider = new CompositeProvider();
        Assert.False(provider.TryGetMemberKind("Id", out var kind));
        Assert.Equal(MemberKind.Unknown, kind);
        provider.AddKind("Id", MemberKind.PropertyDeclaration);
        Assert.True(provider.TryGetMemberKind("Id", out kind));
        Assert.Equal(MemberKind.PropertyDeclaration, kind);
    }
    [Fact]
    public void AddContains()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("");
        var convertType = compilation.GetTypeByMetadataName("System.Convert");
        Assert.NotNull(convertType);
        var methodName = "ToInt32";
        var method = convertType.GetMembers(methodName).FirstOrDefault() as IMethodSymbol;
        Assert.NotNull(method);
        var provider = new CompositeProvider();
        Assert.True(provider.TryDeclareMethod(methodName, compilation.GetStringSymbol()));
        Assert.True(provider.Contains(methodName));
    }
    [Fact]
    public void TryDeclareMethod()
    {
        var methodName = "SayHello";
        var provider = new CompositeProvider();
        Assert.True(provider.TryDeclareMethod(methodName));
        // 第一次成功,之后失败
        Assert.False(provider.TryDeclareMethod(methodName));
    }
    [Fact]
    public void MethodContains()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("");
        var methodName = "SayHello";
        var provider = new CompositeProvider();
        var parameterSymbol = compilation.GetStringSymbol();
        Assert.False(provider.MethodContains(methodName, parameterSymbol));

        Assert.True(provider.TryDeclareMethod(methodName));
        Assert.True(provider.Contains(methodName));
        // 重载方法匹配参数
        //Assert.False(provider.MethodContains(methodName, member));
        Assert.False(provider.MethodContains(methodName, parameterSymbol));

        // 可以添加重载方法
        Assert.True(provider.TryDeclareMethod(methodName, parameterSymbol));
        //Assert.True(provider.MethodContains(methodName, member));
        Assert.True(provider.MethodContains(methodName, parameterSymbol));
    }
    [Fact]
    public void TryDeclareMethod2()
    {
        var source = @"
public partial class B
{
    public string? Id { get; set;}
    public void Deconstruct(out string? id)
    {
        id = Id;
    }
}
public record A(int? Id, string Name, string B);
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
        var nameProvider = CompositeProvider.Create(destSymbol);
        var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();
        var sourceCount = sourceProperties.Length;
        var generator = SyntaxGenerator.Clone(destType);

        var parameterSymbol0 = compilation.GetNullable(compilation.GetStringSymbol());
        var parameterSymbols = new List<ITypeSymbol>(sourceCount) { parameterSymbol0 };
        var method = SyntaxGenerator.VoidType.Method("Deconstruct")
            .ToBuilder()
            .AddParameter(parameterSymbol0.ToSyntax().Parameter("id").Out())
            .AddExpression(SyntaxFactory.IdentifierName("id").Assign(SyntaxFactory.IdentifierName("Id")));
        foreach (var sourceProperty in sourceProperties)
        {
            var propertyName = sourceProperty.Name;
            if (nameProvider.TryDeclareProperty(propertyName))
            {
                var propertySymbol = sourceProperty.Type;
                parameterSymbols.Add(propertySymbol);
                var property = propertySymbol.ToSyntax()
                    .GetSetProperty(propertyName)
                    .Public();
                generator.AddProperty(property);

                var parameter = SyntaxFactory.IdentifierName(CamelWordRule.FistToLower(propertyName));
                method.AddParameter(propertySymbol.ToSyntax().Parameter(parameter.Identifier).Out())
                    .AddExpression(parameter.Assign(SyntaxFactory.IdentifierName(propertyName)));
            }
        }
        if (nameProvider.TryDeclareMethod("Deconstruct", [.. parameterSymbols]))
            generator.AddMethod(method.End().Public());
        var code = generator.Build().NormalizeWhitespace().ToFullString();
        Assert.Contains("Name", code);
        Assert.Contains("Deconstruct", code);
    }
    [Fact]
    public void Create()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("record User(int Id, string Name);");
        var symbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(symbol);
        var provider = CompositeProvider.Create(symbol);
        Assert.True(provider.Contains("Id"));
        Assert.True(provider.Contains("Name")); 
        Assert.True(provider.Contains("User"));
        Assert.True(provider.Contains("GetHashCode"));
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
        var provider = CompositeProvider.Inherit(symbol);
        Assert.True(provider.Contains("Account"));
        Assert.True(provider.Contains("Name"));
        Assert.True(provider.Contains("Hello"));
        Assert.True(provider.Contains("Balance"));
        Assert.True(provider.Contains("Pay"));
    }
}
