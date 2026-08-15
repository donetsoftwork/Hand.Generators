using GenerateConvertTests.Supports;
using Hand;
using Hand.Documentation;
using Hand.Members;
using Hand.ParseXml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateConvertTests.Enums;

public class CommentXmlTests
{
    [Fact]
    public void SingleLineDocumentationCommentTrivia()
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
            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                if (trivia.GetStructure() is DocumentationCommentTriviaSyntax comment)
                {
                    var items = comment.Content;
                    if (items.Count == 3)
                    {
                        var first = items[0];
                        var summary = items[1];
                        var last = items[2];
                        if (summary is XmlElementSyntax xmlElement)
                        {
                            Console.WriteLine(xmlElement);
                            string[] texts = [first.ToFullString(), xmlElement.ToFullString(), last.ToFullString()];
                            Assert.Equal("/// ", texts[0]);
                            Assert.Equal("<summary> This is an xml doc comment </summary>", texts[1]);
                            Assert.Equal("\r\n", texts[2]);
                        }
                    }
                }
                   
            }
        }
    }
    [Fact]
    public void SingleLineDocumentationCommentTrivia2()
    {
        var tree = CSharpSyntaxTree.ParseText(@"
/// <summary>
/// This is an xml doc comment 
/// </summary>
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
            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                if (trivia.GetStructure() is DocumentationCommentTriviaSyntax comment)
                {
                    var items = comment.Content;
                    if (items.Count == 3)
                    {
                        var first = items[0];
                        var summary = items[1];
                        var last = items[2];
                        if (summary is XmlElementSyntax xmlElement)
                        {
                            Console.WriteLine(xmlElement);
                            string[] texts = [first.ToFullString(), xmlElement.ToFullString(), last.ToFullString()];
                            Assert.Equal("/// ", texts[0]);
                            Assert.Equal("<summary>\r\n/// This is an xml doc comment \r\n/// </summary>", texts[1]);
                            Assert.Equal("\r\n", texts[2]);
                        }
                    }
                }

            }
        }
    }
    [Fact]
    public void SingleLineDocumentationCommentTrivia3()
    {
        var tree = CSharpSyntaxTree.ParseText(@"
/// <summary>
/// This is an xml doc comment 
/// </summary>
/// <param name=""value"">This is an int param</param>
class C(int value)
{
    public int Value { get; } = value;
}");
        var root = (CompilationUnitSyntax)tree.GetRoot();
        var classNode = (ClassDeclarationSyntax)(root.Members.First());

        var trivias = classNode.GetLeadingTrivia();
        var enumerator = trivias.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var trivia = enumerator.Current;
            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                if (trivia.GetStructure() is DocumentationCommentTriviaSyntax comment)
                {
                    var items = comment.Content;
                    if (items.Count == 5)
                    {
                        var first = items[0];
                        var summary = items[1];
                        var separator = items[2];
                        var param = items[3];
                        var last = items[4];
                        if (summary is XmlElementSyntax summaryElement && param is XmlElementSyntax paramElement)
                        {
                            Console.WriteLine(summaryElement);
                            string[] texts = [first.ToFullString(), summaryElement.ToFullString(), separator.ToFullString(), paramElement.ToFullString(), last.ToFullString()];
                            Assert.Equal("/// ", texts[0]);
                            Assert.Equal("<summary>\r\n/// This is an xml doc comment \r\n/// </summary>", texts[1]);
                            Assert.Equal("\r\n/// ", texts[2]);
                            Assert.Equal("<param name=\"value\">This is an int param</param>", texts[3]);
                            Assert.Equal("\r\n", texts[4]);
                        }
                    }
                }

            }
        }
    }
    ///// <summary>
    ///// This is an xml doc comment 
    ///// </summary>
    ///// <param name="value">This is an int param</param>
    //class C(int value)
    //{
    //    public int Value { get; } = value;
    //}
    //[Fact]
    //public void XmlMultiLineElement()
    //{
    //    XmlNodeSyntax first = SyntaxFactory.XmlText("first");
    //    XmlNodeSyntax separator = SyntaxFactory.XmlText("\r\n");
    //    XmlNodeSyntax second = SyntaxFactory.XmlText("second");
    //    var multiLineElement = SyntaxFactory.XmlMultiLineElement("summary", SyntaxFactory.List([first, separator, second]));
    //    var code = multiLineElement.NormalizeWhitespace().ToFullString();
    //    Assert.Contains("summary", code);
    //}
    [Fact]
    public void GetCommentXml()
    {
        //var options = CSharpParseOptions.Default.WithDocumentationMode(Microsoft.CodeAnalysis.DocumentationMode.Diagnose);
        var tree = CSharpSyntaxTree.ParseText(@"
    /// <summary>
    /// C
    /// </summary>
    /// <param name=""Id"">Id</param>
    /// <param name=""Name"">Name</param>
    record C(int Id, string Name);");
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
    [Fact]
    public void Parse()
    {
        var sourceCode = @"
            /// <summary>
            /// C
            /// </summary>
            /// <param name=""Id"">Id</param>
            /// <param name=""Name"">Name</param>
            record C(int Id, string Name);";
        var compilation = SyntaxTreeDriver.CreateDefaultDriver()
            .Compile(sourceCode);
        INamedTypeSymbol? classSymbol = compilation.GetTypeByMetadataName("C");
        Assert.NotNull(classSymbol);
        string? xml = classSymbol.GetDocumentationCommentXml();
        Assert.NotNull(xml);
        string summary = CommentParser.GetSummary(xml);
        Assert.Equal("C", summary);
        Comment comment = CommentParser.Instance.Get(xml);
        Assert.Equal("C", comment.Summary.Trim());
        Assert.Equal(2, comment.Params.Count);
        Comment comment2 = CommentParser.Comment(classSymbol);
        Assert.Equal("C", comment2.Summary.Trim());
        Assert.Equal(2, comment2.Params.Count);
    }
}
