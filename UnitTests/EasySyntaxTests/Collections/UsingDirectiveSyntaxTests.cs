using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests.Collections;

public class UsingDirectiveSyntaxTests
{
    [Fact]
    public void Parse()
    {
        string sourceCode = @"
using System;
using T = System.DateTime;
using IntList = System.Collections.Generic.List<int>;
";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var usings = syntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>()
            .ToArray();
        Assert.Equal(3, usings.Length);
        var first = usings[0].Name;
        Assert.NotNull(first);
        Assert.True(first.IsKind(SyntaxKind.IdentifierName));
        var second = usings[1].Name;
        Assert.NotNull(second);
        var last = usings[2].Name;
        Assert.NotNull(last);
        Assert.True(last.IsKind(SyntaxKind.QualifiedName));
        if (last is not QualifiedNameSyntax qualified)
        {
            Assert.Fail();
            return;
        }
        var qualifiedCode = qualified.GetHashCode();
        var left = qualified.Left;
        Assert.True(left.IsKind(SyntaxKind.QualifiedName));
        var right = qualified.Right;
        Assert.True(right.IsKind(SyntaxKind.GenericName));
    }
}
