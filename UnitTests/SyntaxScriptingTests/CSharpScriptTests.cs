using Hand;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace SyntaxScriptingTests;

public class CSharpScriptTests
{
    [Fact]
    public async Task EvaluateAsync()
    {
        var result = await CSharpScript.EvaluateAsync<int>("2+3");
        Assert.Equal(5, result);
    }

    private static readonly string[] expected = ["Helo A", "Helo B"];

    [Fact]
    public async Task Script()
    {
        var a = ScriptA();
        var b = ScriptB(a);
        var source = @"return [new A().Print(), new B().Print()];";
        var driver = SyntaxTreeDriver.CreateScriptDriver()
            .Reference(typeof(Console).Assembly);

        var script = driver.CreateScript<string[]>(source, previous: b);
        var results = await script.ExecuteAsync();
        Assert.Equal(expected, results);
    }

    private static CSharpCompilation ScriptA()
    {
        var source = @"public class A {
        public virtual string Print()
            => ""Helo A"";
    }";
        return SyntaxTreeDriver.DefaultDriver.ScriptCompile(source);
    }
    private static CSharpCompilation ScriptB(CSharpCompilation a)
    {
        var source = @"public class B : A {
        public override string Print()
            => ""Helo B"";
    }";
        return SyntaxTreeDriver.DefaultDriver.ScriptCompile(source, previous: a);
    }
}
