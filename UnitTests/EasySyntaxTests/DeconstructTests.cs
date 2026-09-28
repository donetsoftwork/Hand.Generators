using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace EasySyntaxTests;

public class DeconstructTests
{
    [Fact]
    public void Int()
    {
        var tuple = SyntaxGenerator.Tuple([SyntaxGenerator.Literal(1), SyntaxGenerator.Literal(2)]);
        var expression = tuple.Deconstruct(SyntaxGenerator.IntType, "x", "y");
        Assert.Equal("int (x, y) = (1, 2)", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Int0()
    {
        var tuple = SyntaxFactory.TupleExpression([
            SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression, 
                    SyntaxFactory.Literal(1))),
            SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression, 
                    SyntaxFactory.Literal(2)))]);
        var expression = SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression, 
            SyntaxFactory.DeclarationExpression(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.ParenthesizedVariableDesignation([
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")),
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))])), 
            tuple);
        Assert.Equal("int (x, y) = (1, 2)", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Var()
    {
        var tuple = SyntaxGenerator.Tuple([SyntaxGenerator.Literal(1), SyntaxGenerator.Literal(2)]);
        var expression = tuple.Deconstruct(SyntaxGenerator.VarType, "x", "y");
        Assert.Equal("var(x, y) = (1, 2)", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Var0()
    {
        var tuple = SyntaxFactory.TupleExpression([
            SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(1))),
            SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(2)))]);
        var expression = SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.DeclarationExpression(
                SyntaxFactory.IdentifierName(
                    SyntaxFactory.Identifier(
                        SyntaxFactory.TriviaList(),
                        SyntaxKind.VarKeyword,
                        "var",
                        "var",
                        SyntaxFactory.TriviaList())),
                SyntaxFactory.ParenthesizedVariableDesignation([
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")),
                    SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))])),
            tuple);
        Assert.Equal("var(x, y) = (1, 2)", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Tuple()
    {
        var tuple = SyntaxGenerator.Tuple([SyntaxGenerator.Literal(1), SyntaxGenerator.Literal(2)]);
        var expression = tuple.Deconstruct("x", "y");
        Assert.Equal("(x, y) = (1, 2)", expression.NormalizeWhitespace().ToFullString());
    }
    [Fact]
    public void Tuple0()
    {
        var tuple = SyntaxFactory.TupleExpression([
            SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(1))),
            SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(2)))]);
        var expression = SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxGenerator.Tuple([
                SyntaxFactory.IdentifierName("x"), 
                SyntaxFactory.IdentifierName("y")]),
            tuple);
        Assert.Equal("(x, y) = (1, 2)", expression.NormalizeWhitespace().ToFullString());
    }
}
