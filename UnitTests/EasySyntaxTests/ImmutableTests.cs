using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class ImmutableTests
{
    [Fact]
    public void Record()
    {
        var record = SyntaxGenerator.RecordDeclaration("Person")
            .WithSemicolonToken();
        var record2 = record.AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
        var code = record.NormalizeWhitespace().ToFullString();
        var code2 = record2.NormalizeWhitespace().ToFullString();
        Assert.NotEqual(code, code2);
    }

    [Fact]
    public void List()
    {
        var record = SyntaxGenerator.RecordDeclaration("Person")
            .WithSemicolonToken();
        var list = new SeparatedSyntaxList<ParameterSyntax>
        {
            SyntaxGenerator.IntType.Parameter("Id")
        };
        list.Add(SyntaxGenerator.StringType.Parameter("Name"));
        Assert.Empty(list);
        var record2 = record.WithParameterList(SyntaxFactory.ParameterList(list));
        var code = record2.NormalizeWhitespace().ToFullString();
        Assert.DoesNotContain("Id", code);
        Assert.DoesNotContain("Name", code);
    }
}
