using Hand;
using Hand.Naming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NamingTests;

public class MethodContainerTests
{
    [Fact]
    public void AddContains()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("");
        var convertType = compilation.GetTypeByMetadataName("System.Convert");
        Assert.NotNull(convertType);
        var methodName = "ToInt32";
        var method = convertType.GetMembers(methodName).FirstOrDefault() as IMethodSymbol;
        Assert.NotNull(method);

        var container = new MethodContainer();
        Assert.True(container.TryDeclareMethod(methodName, compilation.GetStringSymbol()));
        Assert.True(container.Contains(methodName));
    }
    [Fact]
    public void TryDeclareMethod()
    {
        var methodName = "SayHello";

        var container = new MethodContainer();
        Assert.True(container.TryDeclareMethod(methodName));
        // 第一次成功,之后失败
        Assert.False(container.TryDeclareMethod(methodName));
    }
    [Fact]
    public void MethodContains()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("");
        var methodName = "SayHello";
        var container = new MethodContainer();
        var parameterSymbol = compilation.GetStringSymbol();
        Assert.False(container.MethodContains(methodName, parameterSymbol));


        Assert.False(container.MethodContains(methodName, parameterSymbol));

        Assert.True(container.TryDeclareMethod(methodName));
        Assert.True(container.Contains(methodName));
        // 重载方法匹配参数
        //Assert.False(container.MethodContains(methodName, member));
        Assert.False(container.MethodContains(methodName, parameterSymbol));
        // 可以添加重载方法
        Assert.True(container.TryDeclareMethod(methodName, parameterSymbol));
        //Assert.True(container.MethodContains(methodName, member));
        Assert.True(container.MethodContains(methodName, parameterSymbol));
    }
    [Fact]
    public void AddMethod()
    {
        var source = @"
public partial class Calculator
{
    public static int Add(int a, int b) => a + b;
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver();
        SyntaxTree syntaxTree = driver.Parse(source);
        var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(destType);
        var compilation = driver.Compile(syntaxTree);
        var destSymbol = compilation.GetTypeByMetadataName("Calculator");
        Assert.NotNull(destSymbol);
        var nameContainer = MethodContainer.Create(destSymbol);
        var generator = SyntaxGenerator.Clone(destType);
        var methodName = "Add";
        Assert.True(nameContainer.Contains(methodName));
        var methodSymbol = compilation.GetDecimalSymbol();
        INamedTypeSymbol[] parameterSymbols = [methodSymbol, methodSymbol];
        if (nameContainer.TryDeclareMethod(methodName, parameterSymbols))
        {
            var methodType = methodSymbol.ToSyntax();
            var a = SyntaxFactory.IdentifierName("a");
            var b = SyntaxFactory.IdentifierName("b");
            var method = methodType.Method(methodName, [methodType.Parameter(a.Identifier), methodType.Parameter(b.Identifier)])
                .ToBuilder()
                .Return(a.Add(b))
                .Public()
                .Static();
            generator.AddMethod(method);
        }
        var code = generator.Build().NormalizeWhitespace().ToFullString();
        Assert.Contains(methodName, code);
    }
    [Fact]
    public void Create()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("record User(int Id, string Name);");
        var symbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(symbol);
        var container = MethodContainer.Create(symbol);
        Assert.True(container.Contains("GetHashCode"));
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
        var provider = MethodContainer.Inherit(symbol);
        // MethodContainer只支持方法
        Assert.False(provider.Contains("Name"));
        Assert.True(provider.Contains("Hello"));
        Assert.True(provider.Contains("Pay"));
    }
}
