using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxScriptingTests;

public class ScriptLoadContextTests
{
    private static readonly string[] expected = ["Helo A", "Helo B"];

    [Fact]
    public async Task Script()
    {
        var a = ScriptA();
        var b = ScriptB(a);
        var source = @"return [new A().Print(), new B().Print()];";
        var script = SyntaxTreeDriver.ScriptDriver.CreateScript<string[]>(source, previous: b);
        var results = await script.ExecuteAsync();
        Assert.Equal(expected, results);
    }

    private static CSharpCompilation ScriptA()
    {
        var source = @"public class A {
        public virtual string Print()
            => ""Helo A"";
    }";
        return SyntaxTreeDriver.ScriptDriver.ScriptCompile(source);
    }
    private static CSharpCompilation ScriptB(CSharpCompilation a)
    {
        var source = @"public class B : A {
        public override string Print()
            => ""Helo B"";
    }";
        return SyntaxTreeDriver.ScriptDriver.ScriptCompile(source, previous: a);
    }

    [Fact]
    public async Task Module()
    {
        using var context = new ScriptLoadContext();
        var a = ModuleA(context);
        var b = ModuleB(context, a);
        // https://blog.csdn.net/mzl87/article/details/120983208
        var source = @"return [new A().Print(), new B().Print()];";
        var driver = SyntaxTreeDriver.CreateScriptDriver()
            .Reference(a)
            .Reference(b);

        var compilation = driver.ScriptCompile(source, returnType: typeof(string[]));
        var script = new SyntaxTreeScript<string[]>(compilation, context);
        var results = await script.ExecuteAsync();
        Assert.Equal(expected, results);
    }

    private static MetadataReference ModuleA(ScriptLoadContext context)
    {
        var source = @"public class A {
        public virtual string Print()
            => ""Helo A"";
    }";
        var compilation = SyntaxTreeDriver.CreateDriver()
            .Reference<object>()
            .Compile(source);
        var assembly = context.GetAssembly(compilation);
        return context.CheckReference(assembly)!;
    }
    private static MetadataReference ModuleB(ScriptLoadContext context, MetadataReference a)
    {
        var source = @"public class B : A {
        public override string Print()
            => ""Helo B"";
    }";
        var compilation = SyntaxTreeDriver.CreateDriver()
            .Reference<object>()
            .Reference(a)
            .Compile(source);
        var assembly = context.GetAssembly(compilation);
        return context.CheckReference(assembly)!;
    }
}
