using Hand;
using Hand.Documentation;
using Hand.Members;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationTests;

/// <summary>
/// 注释测试
/// </summary>
public class CommentTests
{
    [Fact]
    public void SingleLineComment()
    {
        SyntaxTrivia comment = SyntaxFactory.Comment("// singleLine Comment");
        Assert.True(comment.IsKind(SyntaxKind.SingleLineCommentTrivia));

    }
    [Fact]
    public void MultiLineComment()
    {
        SyntaxTrivia comment = SyntaxFactory.Comment("/* MultiLine\r\n Comment */");
        Assert.True(comment.IsKind(SyntaxKind.MultiLineCommentTrivia));
    }
    [Fact]
    public void Default()
    {
        SyntaxTrivia comment = SyntaxFactory.Comment("This is a Comment");
        Assert.True(comment.IsKind(SyntaxKind.SingleLineCommentTrivia));
    }
    [Fact]
    public void Element()
    {
        SyntaxList<XmlNodeSyntax> content = [SyntaxFactory.XmlText("用户名")];
        XmlElementSyntax summary = SyntaxFactory.XmlElement("summary", content);
        Assert.Equal("<summary>", summary.StartTag.ToFullString());
        Assert.Equal("</summary>", summary.EndTag.ToFullString());
        Assert.Equal("<summary>用户名</summary>", summary.ToFullString());
    }
    [Fact]
    public void Attribute()
    {
        XmlNameAttributeSyntax attribute = SyntaxFactory.XmlNameAttribute("id");
        Assert.Equal(" name=\"id\"", attribute.ToFullString());
    }
    [Fact]
    public void ElementWithAttribute()
    {
        SyntaxList<XmlNodeSyntax> content = [SyntaxFactory.XmlText("用户")];
        XmlElementSyntax element = SyntaxFactory.XmlElement("param", content);
        XmlNameAttributeSyntax attribute = SyntaxFactory.XmlNameAttribute("userId");
        element = element.WithStartTag(element.StartTag.AddAttributes(attribute));
        Assert.Equal("<param name=\"userId\">", element.StartTag.ToFullString());
        Assert.Equal("</param>", element.EndTag.ToFullString());
        Assert.Equal("<param name=\"userId\">用户</param>", element.ToFullString());
    }
    [Fact]
    public void Summary()
    {
        XmlElementSyntax summary = SyntaxFactory.XmlSummaryElement(SyntaxFactory.XmlText("用户名"));
        Assert.Equal("<summary>", summary.StartTag.ToFullString());
        Assert.Equal("</summary>", summary.EndTag.ToFullString());
        Assert.Equal("<summary>用户名</summary>", summary.ToFullString());

        DocumentationCommentTriviaSyntax documentation = SyntaxGenerator.CreateDocumentation(summary);
        Assert.True(documentation.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        Assert.Equal("/// <summary>用户名</summary>\r\n", documentation.ToFullString());
    }
    [Fact]
    public void SummaryWithNewLine()
    {
        var line = XmlNewLine(true);
        SyntaxList<XmlNodeSyntax> context = [line, SyntaxFactory.XmlText("用户名"), line];
        XmlElementSyntax summary = SyntaxFactory.XmlSummaryElement(context);
        Assert.Equal("\r\n/// ", line.ToFullString());
        Assert.Equal("<summary>", summary.StartTag.ToFullString());
        Assert.Equal("</summary>", summary.EndTag.ToFullString());
        Assert.Equal("<summary>\r\n/// 用户名\r\n/// </summary>", summary.ToFullString());
        DocumentationCommentTriviaSyntax documentation = SyntaxGenerator.CreateDocumentation(summary);
        Assert.True(documentation.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        Assert.Equal("/// <summary>\r\n/// 用户名\r\n/// </summary>\r\n", documentation.ToFullString());

        static XmlTextSyntax XmlNewLine(bool continueComment)
            => SyntaxFactory.XmlText(SyntaxFactory.XmlTextNewLine("\r\n", continueComment));
    }

    [Fact]
    public void Multi()
    {
        var separator = XmlNewLine(true);
        XmlElementSyntax summary = XmlSummary("实体主键", separator);
        XmlElementSyntax typeparam = NamedXmlElement("typeparam", "TKey", "主键类型");
        XmlElementSyntax param = NamedXmlElement("param", "key", "主键");
        XmlNodeSyntax[] content = [summary, separator, typeparam, separator, param, XmlNewLine(false)];
        DocumentationCommentTriviaSyntax documentation = SyntaxFactory.DocumentationComment(content);
        var code = documentation.ToFullString();
        Assert.Contains("<summary>", code);


        static XmlElementSyntax NamedXmlElement(string elementName, string name, string text)
        {
            SyntaxList<XmlNodeSyntax> content = [SyntaxFactory.XmlText(text)];
            XmlElementSyntax element = SyntaxFactory.XmlElement(elementName, content);
            XmlNameAttributeSyntax attribute = SyntaxFactory.XmlNameAttribute(name);
            return element.WithStartTag(element.StartTag.AddAttributes(attribute));
        }
        static XmlElementSyntax XmlSummary(string summary, XmlTextSyntax line)
        {
            SyntaxList<XmlNodeSyntax> summaryContext = [line, SyntaxFactory.XmlText(summary), line];
            return SyntaxFactory.XmlSummaryElement(summaryContext);
        }
        static XmlTextSyntax XmlNewLine(bool continueComment)
            => SyntaxFactory.XmlText(SyntaxFactory.XmlTextNewLine("\r\n", continueComment));
    }
    [Fact]
    public void CreateDocumentation()
    {
        var comment = new Comment() 
        {
            Summary = "实体主键",
            TypeParams = { { "TKey", "主键类型" } },
            Params = { { "key", "主键" } }
        };
        DocumentationCommentTriviaSyntax? documentation = SyntaxGenerator.CreateDocumentation(comment);
        Assert.NotNull(documentation);
        var code = documentation.ToFullString();
        Assert.Contains("<summary>", code);
    }
    [Fact]
    public void CreateDocumentationBySingleLineComment()
    {
        SyntaxTrivia comment = SyntaxFactory.Comment("/// <summary>This is  comment</summary>");
        var method = SyntaxGenerator.VoidType.Method("Test")
            .WithBody(SyntaxFactory.Block())
            .WithLeadingTrivia(comment);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("<summary>", code);
    }
    [Fact]
    public void Map()
    {
        var sourceCode = @"
            namespace ExampleNamespace;

            /// <summary>
            /// 用户
            /// </summary>
            /// <param name=""UserName"">用户名</param>
            public record User(string UserName);
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().LastOrDefault();
        Assert.NotNull(classDeclaration);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        Assert.NotNull(symbol);
        var sourceSymbol = compilation.GetTypeByMetadataName("ExampleNamespace.User");
        Assert.NotNull(sourceSymbol);
        var generator = SyntaxGenerator.Clone(classDeclaration);
        foreach (var propertySymbol in SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol))
        {
            string propertySummary = CommentParser.GetSummary(propertySymbol);
            var propertyType = propertySymbol.Type.ToSyntax();
            var property = propertyType.GetSetProperty(propertySymbol.Name)
                .Public()
                .WithSummary(propertySummary);
            generator.AddProperty(property);
        }
        string sourceSummary = CommentParser.GetSummary(sourceSymbol);
        generator.Apply(type => type.WithSummary(sourceSummary));
        var code = generator.Build().NormalizeWhitespace().ToFullString();
        Assert.Contains("<summary>", code);
    }
    ///// <summary>
    ///// 用户
    ///// </summary>
    ///// <param name="UserName">用户名</param>
    //public record User(string UserName);
    /////<summary>
    /////用户
    /////</summary>
    //partial class UserDTO
    //{
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string UserName { get; set; }
    //}
/// <summary>
/// 实体主键
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
/// <param name="key">主键</param>
public class EntityKey<TKey>(TKey key)
    {
        /// <summary>
        /// 主键
        /// </summary>
        public TKey Key { get; } = key;
    }
}
