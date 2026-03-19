using Hand;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace SyntaxScriptingTests;

public class SyntaxTreeScriptTests
{
    [Fact]
    public void Using()
    {
        var service = SyntaxTreeScript.Create()
            .Using("System");
        Assert.Single(service.Usings);
    }
    [Fact]
    public void Reference()
    {
        var service = SyntaxTreeScript.Create()
            .Reference<DateTime>();
        Assert.Single(service.References);
    }
    [Fact]
    public void CreateDefault()
    {
        var service = SyntaxTreeScript.CreateDefault();
        Assert.Single(service.Usings);
        Assert.NotEmpty(service.References);
    }
    [Fact]
    public void Default()
    {
        var service = SyntaxTreeScript.Default;
        Assert.Single(service.Usings);
        Assert.NotEmpty(service.References);
    }
    [Fact]
    public void Parse()
    {
        var source = "public record UserCreateTime(DateTime Original);";
        var service = SyntaxTreeScript.Create()
            .Using("System");
        var tree = service.Parse(source);
        Assert.NotNull(tree);
    }
    [Fact]
    public void Compile()
    {
        var source = "public record UserCreateTime(DateTime Original);";
        var service = SyntaxTreeScript.Create()
            .Using("System");
        var compilation = service.Compile(source);
        var type = compilation.GetTypeByMetadataName("UserCreateTime");
        Assert.NotNull(type);
    }
    [Fact]
    public void Generate()
    {
        var source = "public partial class Greeting;";
        var service = SyntaxTreeScript.Create()
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
