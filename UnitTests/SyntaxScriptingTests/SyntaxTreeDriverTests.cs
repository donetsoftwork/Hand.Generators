using Hand;

namespace SyntaxScriptingTests;

public class SyntaxTreeDriverTests
{
    [Fact]
    public void Using()
    {
        var service = SyntaxTreeDriver.CreateDriver()
            .Using("System");
        Assert.Single(service.Usings);
    }
    [Fact]
    public void Reference()
    {
        var service = SyntaxTreeDriver.CreateDriver()
            .Reference<DateTime>();
        Assert.Single(service.References);
    }
    [Fact]
    public void CreateDefault()
    {
        var service = SyntaxTreeDriver.CreateDefaultDriver();
        Assert.Single(service.Usings);
        Assert.NotEmpty(service.References);
    }
    [Fact]
    public void Default()
    {
        var service = SyntaxTreeDriver.DefaultDriver;
        Assert.Single(service.Usings);
        Assert.NotEmpty(service.References);
    }
    [Fact]
    public void Parse()
    {
        var source = "public record UserCreateTime(DateTime Original);";
        var service = SyntaxTreeDriver.CreateDriver()
            .Using("System");
        var tree = service.Parse(source);
        Assert.NotNull(tree);
    }
    [Fact]
    public void Compile()
    {
        var source = "public record UserCreateTime(DateTime Original);";
        var service = SyntaxTreeDriver.CreateDriver()
            .Using("System");
        var compilation = service.Compile(source);
        var type = compilation.GetTypeByMetadataName("UserCreateTime");
        Assert.NotNull(type);
    }
    [Fact]
    public void ScriptCompile()
    {
        var compilation = SyntaxTreeDriver.ScriptDriver.ScriptCompile("1+2");
        var type = compilation.GetTypeByMetadataName("Script");
        Assert.NotNull(type);
    }
    [Fact]
    public void Generate()
    {
        var source = "public partial class Greeting;";
        var service = SyntaxTreeDriver.CreateDriver()
            .Using("System");
        var result = service.Generate<HelloGenerator>(source)
            .GetRunResult();
        var tree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(tree);
        var diagnostics = result.Diagnostics;
        Assert.Empty(diagnostics);
        var @code = tree.ToString();
        Assert.NotEmpty(@code);
    }
}
