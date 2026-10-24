using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class LeadingTriviaTests
{
    [Fact]
    public void GetDocumentationCommentTrivia()
    {
        string code = @"
    /// <summary>
    /// This is a summary.
    /// </summary>
    /// <param name=""Name""></param>
    class UserName(string name)
    {
        /// <inheritdoc cref=""Name"" path=""/summary"" />
        protected string _name = name;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
            => _name;
        /// <inheritdoc />
        public override string ToString()
            => Name;
    }";

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;
        Assert.NotNull(root);
        var type = root.Members.OfType<ClassDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(type);
        var typeDocumentation = type.GetDocumentation();
        Assert.NotNull(typeDocumentation);
        var summary = typeDocumentation.GetSummary();
        Assert.NotNull(summary);
        var summary2 = type.GetSummary(); 
        Assert.NotNull(summary2);
        var method = type.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(method);
        var methodDocumentation = method.GetDocumentation();
        Assert.NotNull(methodDocumentation);
    }
    [Fact]
    public void GetDocumentationCommentTrivia0()
    {
        string code = @"
        /// <summary>
        /// This is a summary.
        /// </summary>
        public class MyClass { }";

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;
        Assert.NotNull(root);
        var classDeclaration = root.Members.FirstOrDefault() as ClassDeclarationSyntax;
        Assert.NotNull(classDeclaration);
        var leadingTrivia = classDeclaration.GetLeadingTrivia();
        foreach (var trivia in leadingTrivia)
        {
            //if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
            //{
            //    Console.WriteLine(trivia.ToString());
            //}
            if(trivia.HasStructure && trivia.GetStructure() is DocumentationCommentTriviaSyntax documentation)
            {
                foreach (var node in documentation.Content)
                {
                    if(node is XmlElementSyntax xmlElement && xmlElement.StartTag.Name.LocalName.ValueText == "summary")
                        Console.WriteLine(xmlElement.ToString());
                }
            }
        }
    }
    [Fact]
    public void XmlSummaryElement0()
    {
        var summary = SyntaxFactory.XmlSummaryElement(
            SyntaxFactory.XmlText(SyntaxFactory.XmlTextNewLine("\r\n", true)),
            SyntaxFactory.XmlText("This is a summary."),
            SyntaxFactory.XmlText(SyntaxFactory.XmlTextNewLine("\r\n", true)));
        var documentation = SyntaxFactory.DocumentationComment(summary, SyntaxFactory.XmlText(SyntaxFactory.XmlTextNewLine("\r\n", false)));
        var type = SyntaxFactory.ClassDeclaration("MyClass")
            .WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Trivia(documentation)));
        var code = type/*.NormalizeWhitespace()*/.ToFullString();
        Assert.NotNull(code);
    }
    [Fact]
    public void XmlSummaryElement()
    {
        var type = SyntaxFactory.ClassDeclaration("MyClass")
            .WithSummary("This is a summary.");
        var code = type.ToFullString();
        Assert.NotNull(code);
    }
}
