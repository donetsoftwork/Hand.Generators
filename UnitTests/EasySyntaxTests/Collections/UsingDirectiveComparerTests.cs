using Hand;
using Hand.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace EasySyntaxTests.Collections;

public class UsingDirectiveComparerTests
{
    private readonly UsingDirectiveComparer _comparer = UsingDirectiveComparer.Instance;

    [Fact]
    public void IdentifierEquals()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("using System;");
        var usingDirective = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .FirstOrDefault();
        Assert.NotNull(usingDirective);
        var usingDirective2 = SyntaxGenerator.SystemDirective;
        Assert.True(UsingDirectiveComparer.Equals(usingDirective, usingDirective2));
    }
    [Fact]
    public void QualifiedEquals()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("using System.Linq;");
        var usingDirective = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .FirstOrDefault();
        Assert.NotNull(usingDirective);
        var usingDirective2 = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Linq").Qualify("System"));
        Assert.True(UsingDirectiveComparer.Equals(usingDirective, usingDirective2));
    }
    [Fact]
    public void GenericEquals()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("using IntList = System.Collections.Generic.List<int>;");
        var usingDirective = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .FirstOrDefault();
        Assert.NotNull(usingDirective);
        var genericType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType)
            .Qualifies("System", "Collections", "Generic");
        var alias = SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("IntList"));
        var usingDirective2 = SyntaxFactory.UsingDirective(alias, genericType);
        Assert.True(UsingDirectiveComparer.Equals(usingDirective, usingDirective2));
    }
    [Theory]
    [InlineData("using System;")]
    [InlineData("using System.Linq;")]
    [InlineData("using System.Collections;")]
    [InlineData("using System.Collections.Generic;")]
    [InlineData("using IntList = System.Collections.Generic.List<int>;")]
    public void Consistent(string sourceCode)
    {
        // 哈希计算要稳定,相同东西计算多次要一致
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var usingDirective = syntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>()
            .FirstOrDefault();
        Assert.NotNull(usingDirective);
        var expected = _comparer.GetHashCode(usingDirective);
        for (int i = 0; i < 10; i++)
        {
            var result = _comparer.GetHashCode(usingDirective);
            Assert.Equal(expected, result);
        }
    }
    [Fact]
    public void Distribution()
    {
        // 哈希分布要均匀,减少碰撞
        string sourceCode = @"
using Hand;
using Hand.Builders;
using Hand.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Media;
using System.Messaging;
using System.Net;
using System.Net.Cache;
using System.Net.Configuration;
using System.Net.Mime;
using System.Net.Networkinformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Resources.Tools;
using System.Runtime.CompilerServices;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Transactions;
using System.Web;
using System.Web.Mobile;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.XML;
using T = System.DateTime;
using IntList = System.Collections.Generic.List<int>;
";
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var usingDirectives = syntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>()
            .ToArray();
        var usingDirectivesHashCodes = usingDirectives.Select(item => new NamedCode(item.ToFullString(), _comparer.GetHashCode(item)))
            .GroupBy(item => item.Code)
            .OrderByDescending(item => item.Count())
            .ToArray();
        Assert.True(usingDirectivesHashCodes.Length > usingDirectives.Length / 2);
    }
    internal record NamedCode(string Name, int Code);

}
