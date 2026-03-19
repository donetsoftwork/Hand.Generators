using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class SyntaxGeneratorTests
{
    [Fact]
    public void CloneClass()
    {
        var @class = SyntaxFactory.ClassDeclaration("UserId")
            .Partial();
        var code0 = @class.NormalizeWhitespace().ToFullString();
        var generator = SyntaxGenerator.Clone(@class);
        var code = generator.Build().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void CloneRecord()
    {
        var record = SyntaxGenerator.RecordDeclaration("UserId")
            .Partial()
            .WithSemicolonToken();
        var code0 = record.NormalizeWhitespace().ToFullString();
        var generator = SyntaxGenerator.Clone(record);
        var code = generator.Build().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void CloneStructDeclaration()
    {
        var record = SyntaxGenerator.RecordStructDeclaration("UserId")
            .Partial();
        var code0 = record.NormalizeWhitespace().ToFullString();
        var generator = SyntaxGenerator.Clone(record);
        var code = generator.Build().ToFullString();
        Assert.Equal(code0, code);
    }
}
