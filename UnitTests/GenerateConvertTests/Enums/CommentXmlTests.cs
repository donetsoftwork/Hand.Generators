using GenerateConvertTests.Supports;
using Hand;
using Hand.Documentation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateConvertTests.Enums;

public class CommentXmlTests
{
    [Fact]
    public void GetStructure()
    {
        var tree = CSharpSyntaxTree.ParseText(@"
/// <summary> This is an xml doc comment </summary>
class C
{
}");
        var root = (CompilationUnitSyntax)tree.GetRoot();
        var classNode = (ClassDeclarationSyntax)(root.Members.First());

        var trivias = classNode.GetLeadingTrivia();
        var enumerator = trivias.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var trivia = enumerator.Current;
            if (trivia.Kind().Equals(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                var xml = trivia.GetStructure();
                Console.WriteLine(xml);
            }
        }
    }

    [Fact]
    public void GetCommentXml()
    {
        var options = CSharpParseOptions.Default.WithDocumentationMode(Microsoft.CodeAnalysis.DocumentationMode.Diagnose);
        var tree = CSharpSyntaxTree.ParseText(@"
    /// <summary>
    /// C
    /// </summary>
    /// <param name=""Id"">Id</param>
    /// <param name=""Name"">Name</param>
    record C(int Id, string Name);", options);
        //var root = (CompilationUnitSyntax)tree.GetRoot();
        //var classNode = (ClassDeclarationSyntax)(root.Members.First());

        //var trivias = classNode.GetLeadingTrivia();
        //var xmlCommentTrivia = trivias.FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        //var xml = xmlCommentTrivia.GetStructure();
        //Console.WriteLine(xml);

        //var assembly = typeof(ColumnType).Assembly;
        //var xmlLocation = Path.ChangeExtension(assembly.Location, "xml");
        //var xmlProvider = XmlDocumentationProvider.CreateFromFile(xmlLocation);
        var references = typeof(ColumnType).Assembly.ToReferences(true);
        var compilation = CSharpCompilation.Create("test",
            syntaxTrees: new[] { tree },
            references: references);
        //var classSymbol = compilation.GlobalNamespace.GetTypeMembers("C").Single();
        var classSymbol = compilation.GetTypeByMetadataName("C");
        Assert.NotNull(classSymbol);
        //var docComment = classSymbol.GetDocumentationCommentXml();
        //Assert.NotNull(docComment);
        var summary = CommentParser.GetSummary(classSymbol);
        Assert.NotNull(summary);
        var type = compilation.GetTypeByMetadataName("GenerateConvertTests.Supports.ColumnType");
        Assert.NotNull(type);
        //var xml = type.GetDocumentationCommentXml();
        //Assert.NotNull(xml);
        var summary2 = CommentParser.GetSummary(type);
        Assert.NotNull(summary2);
    }
}
