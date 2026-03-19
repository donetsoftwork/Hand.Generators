using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace EasySyntaxTests;

public class ExpressionTest
{
    [Fact]
    public void Add()
    {
        var a = SyntaxGenerator.Literal(1);
        var b = SyntaxGenerator.Literal(2);
        var expression = a.Add(b);
        var code = expression.ToFullString();
        Assert.Equal("1+2", code);
    }
    [Fact]
    public void Subtract()
    {
        var a = SyntaxGenerator.Literal(2);
        var b = SyntaxGenerator.Literal(1);
        var expression = a.Subtract(b);
        var code = expression.ToFullString();
        Assert.Equal("2-1", code);
    }
    [Fact]
    public void Multiply()
    {
        var a = SyntaxGenerator.Literal(2);
        var b = SyntaxGenerator.Literal(3);
        var expression = a.Multiply(b);
        var code = expression.ToFullString();
        Assert.Equal("2*3", code);
    }
    [Fact]
    public void Divide()
    {
        var a = SyntaxGenerator.Literal(6);
        var b = SyntaxGenerator.Literal(3);
        var expression = a.Divide(b);
        var code = expression.ToFullString();
        Assert.Equal("6/3", code);
    }
    [Fact]
    public void Modulo()
    {
        var a = SyntaxGenerator.Literal(10);
        var b = SyntaxGenerator.Literal(3);
        var expression = a.Modulo(b);
        var code = expression.ToFullString();
        Assert.Equal("10%3", code);
    }
    [Fact]
    public void And()
    {
        var a = SyntaxGenerator.Literal(1);
        var b = SyntaxGenerator.Literal(2);
        var expression = a.And(b);
        var code = expression.ToFullString();
        Assert.Equal("1&2", code);
    }
    [Fact]
    public void Or()
    {
        var a = SyntaxGenerator.Literal(1);
        var b = SyntaxGenerator.Literal(2);
        var expression = a.Or(b);
        var code = expression.ToFullString();
        Assert.Equal("1|2", code);
    }
    [Fact]
    public void Xor()
    {
        var a = SyntaxGenerator.Literal(1);
        var b = SyntaxGenerator.Literal(2);
        var expression = a.Xor(b);
        var code = expression.ToFullString();
        Assert.Equal("1^2", code);
    }
    [Fact]
    public void LeftShift()
    {
        var a = SyntaxGenerator.Literal(1);
        var b = SyntaxGenerator.Literal(2);
        var expression = a.LeftShift(b);
        var code = expression.ToFullString();
        Assert.Equal("1<<2", code);
    }
    [Fact]
    public void RightShift()
    {
        var a = SyntaxGenerator.Literal(1);
        var b = SyntaxGenerator.Literal(2);
        var expression = a.RightShift(b);
        var code = expression.ToFullString();
        Assert.Equal("1>>2", code);
    }
    [Fact]
    public void LogicalAnd()
    {
        var a = SyntaxGenerator.TrueLiteral;
        var b = SyntaxGenerator.TrueLiteral;
        var expression = a.LogicalAnd(b);
        var code = expression.ToFullString();
        Assert.Equal("true&&true", code);
    }
    [Fact]
    public void LogicalOr()
    {
        var a = SyntaxGenerator.TrueLiteral;
        var b = SyntaxGenerator.FalseLiteral;
        var expression = a.LogicalOr(b);
        var code = expression.ToFullString();
        Assert.Equal("true||false", code);
    }
    [Fact]
    public void LogicalNot()
    {
        var read = SyntaxFactory.IdentifierName("reader").Access("Read");
        var expression = read.Invocation().LogicalNot();
        var code = expression.ToFullString();
        Assert.Equal("!reader.Read()", code);
    }
    [Fact]
    public void PreIncrement()
    {
        var i = SyntaxFactory.IdentifierName("i");
        var expression = i.PreIncrement();
        var code = expression.ToFullString();
        Assert.Equal("++i", code);
    }
    [Fact]
    public void PostIncrement()
    {
        var i = SyntaxFactory.IdentifierName("i");
        var expression = i.PostIncrement();
        var code = expression.ToFullString();
        Assert.Equal("i++", code);
    }
    [Fact]
    public void PreDecrement()
    {
        var i = SyntaxFactory.IdentifierName("i");
        var expression = i.PreDecrement();
        var code = expression.ToFullString();
        Assert.Equal("--i", code);
    }
    [Fact]
    public void PostDecrement()
    {
        var i = SyntaxFactory.IdentifierName("i");
        var expression = i.PostDecrement();
        var code = expression.ToFullString();
        Assert.Equal("i--", code);
    }
    [Fact]
    public void Qualified()
    {
        var userName = SyntaxFactory.IdentifierName("Name").Qualified("user");
        var code = userName.ToFullString();
        Assert.Equal("user.Name", code);
    }
    [Fact]
    public void Access()
    {
        var user = SyntaxFactory.IdentifierName("user");
        // 尽量使用Qualified,除非owner是复杂表达式用不了Qualified
        var userName = user.Access("Name");
        var code = userName.ToFullString();
        Assert.Equal("user.Name", code);        
    }
    [Fact]
    public void Score()
    {
        var type = SyntaxFactory.IdentifierName("Student");
        var score = SyntaxFactory.IdentifierName("Score");
        var other = SyntaxFactory.IdentifierName("other");
        var method = SyntaxGenerator.IntType.Method("Compare", type.Parameter(other.Identifier))
            .ToBuilder()
            .Return(score.Subtract(score.Qualified(other)));
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void ConditionalAccess()
    {
        var user = SyntaxFactory.IdentifierName("user");
        var userName = user.ConditionalAccess("Name");
        var code = userName.ToFullString();
        Assert.Equal("user?.Name", code);
    }
    [Fact]
    public void Interpolation()
    {
        var interpolation = SyntaxGenerator.Interpolation()
            .Add("Hello ")
            .Add(SyntaxFactory.IdentifierName("name"))
            .Add("!")
            .Build();
        var code = interpolation.ToFullString();
        Assert.Equal("$\"Hello {name}!\"", code);

        InterpolatedStringContentSyntax[] contents = [
            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                        SyntaxTriviaList.Empty,
                        SyntaxKind.InterpolatedStringTextToken,
                        "Hello ",
                        "Hello ",
                        SyntaxTriviaList.Empty)),
            SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName("name")),
            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                SyntaxTriviaList.Empty,
                SyntaxKind.InterpolatedStringTextToken,
                "!",
                "!",
                SyntaxTriviaList.Empty))
        ];
        var interpolation0 = SyntaxFactory.InterpolatedStringExpression(SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken), SyntaxGenerator.List(contents));
        var code0 = interpolation0.ToFullString();
        Assert.Equal("$\"Hello {name}!\"", code0);
    }
    [Fact]
    public void InterpolationFormat()
    {
        var interpolation = SyntaxGenerator.Interpolation()
            .Add("Today is: ")
            .Add(SyntaxFactory.IdentifierName("now"), "yyyy-MM-dd")
            .Build();
        var code = interpolation.ToFullString();
        Assert.Equal("$\"Today is: {now:yyyy-MM-dd}\"", code);

        InterpolatedStringContentSyntax[] contents = [
            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                        SyntaxTriviaList.Empty,
                        SyntaxKind.InterpolatedStringTextToken,
                        "Today is: ",
                        "Today is: ",
                        SyntaxTriviaList.Empty)),
            SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName("now"), default,  SyntaxFactory.InterpolationFormatClause(
                SyntaxFactory.Token(SyntaxKind.ColonToken),
                SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.InterpolatedStringTextToken, "yyyy-MM-dd", "yyyy-MM-dd", SyntaxTriviaList.Empty)))
        ];
        var interpolation0 = SyntaxFactory.InterpolatedStringExpression(SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken), SyntaxGenerator.List(contents));
        var code0 = interpolation0.ToFullString();
        Assert.Equal("$\"Today is: {now:yyyy-MM-dd}\"", code0);
    }
}
