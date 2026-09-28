using Hand.Documentation;
using Microsoft.CodeAnalysis.CSharp;

namespace DocumentationTests;

public class CommentParserTests
{
    [Fact]
    public void GetCommentXml()
    {
        var options = CSharpParseOptions.Default.WithDocumentationMode(Microsoft.CodeAnalysis.DocumentationMode.Diagnose);
        var source = @"
/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
/// <param name=""Sex"">性别</param>
public record User(int Id, string Name, int Sex);";
        var tree = CSharpSyntaxTree.ParseText(source, options);
        var compilation = CSharpCompilation.Create("test",[tree]);
        var classSymbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(classSymbol);
        var comment = CommentParser.Comment(classSymbol);
        var id = classSymbol.GetMembers("Id")
            .FirstOrDefault();
        Assert.NotNull(id);
        var idSummary = CommentParser.GetSummary(id);
        Assert.Equal("Id标识", idSummary);
    }
}
