using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class ParseTests
{
    [Fact]
    public void Attribute()
    {
        string sourceCode = @"
using System;

namespace ExampleNamespace
{
    [MyAttribute]
    public class MyClass;
}
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MyAttribute : Attribute;
";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        Assert.NotNull(syntaxTree);
        var attribute = syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
        Assert.NotNull(attribute);
        //var compilation = SyntaxTreeScript
    }
    [Fact]
    public void Switch()
    {
        string sourceCode = @"
namespace System
{
    internal static partial class ConsoleColorExtensions
    {
        internal static global::GenerateConvertTests.Supports.MyColor ToMyColor(this ConsoleColor @this)
        {
            return @this switch
            {
                ConsoleColor.Blue => global::GenerateConvertTests.Supports.MyColor.Blue,
                ConsoleColor.Green => global::GenerateConvertTests.Supports.MyColor.Green,
                ConsoleColor.Red => global::GenerateConvertTests.Supports.MyColor.Red,
                _ => default,
            };
        }
    }
}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        Assert.NotNull(syntaxTree);
        var nodes = syntaxTree.GetRoot().DescendantNodes();
        var @switch = nodes.OfType<SwitchExpressionSyntax>().FirstOrDefault();
        Assert.NotNull(@switch);
        var arm = nodes.OfType<SwitchExpressionArmSyntax>().FirstOrDefault();
        Assert.NotNull(@arm);
    }
    [Fact]
    public void Switch2()
    {
        string sourceCode = @"
namespace Tests
{
    class Grade
    {
        static string ScoreToGrade(int score)
        {
            return score switch
            {
                10 or 9 or 8 or 7 => ""优"",
                6 => ""中"",
                _ => ""差"",
            };
        }
    }
}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        Assert.NotNull(syntaxTree);
        var nodes = syntaxTree.GetRoot().DescendantNodes();
        var @switch = nodes.OfType<SwitchExpressionSyntax>().FirstOrDefault();
        Assert.NotNull(@switch);
        var arm = nodes.OfType<SwitchExpressionArmSyntax>().FirstOrDefault();
        Assert.NotNull(@arm);
    }
}

