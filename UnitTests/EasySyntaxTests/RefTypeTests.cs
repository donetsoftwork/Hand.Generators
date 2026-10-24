using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class RefTypeTests
{
    [Fact]
    public void Ref()
    {
        int x = 10;
        ref int r = ref GetReference(ref x);
        r = 200;
        Assert.Equal(200, x);

        ref int GetReference(ref int data)
        {
            // 可以直接修改 data
            data = 100;
            return ref data; // 返回引用
        }
    }
    [Fact]
    public void RefType()
    {
        string sourceCode = @"
        int x = 10;
        ref int r = ref GetReference(ref x);

        ref int GetReference(ref int data)
        {
            // 可以直接修改 data
            data = 100;
            return ref data; // 返回引用
        }
";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        Assert.NotNull(syntaxTree);
        var types = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<RefTypeSyntax>()
            .ToArray();
        Assert.Equal(2, types.Length);
        var modifiers = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<RefExpressionSyntax>()
            .ToArray();
        Assert.Equal(2, modifiers.Length);
    }
}
