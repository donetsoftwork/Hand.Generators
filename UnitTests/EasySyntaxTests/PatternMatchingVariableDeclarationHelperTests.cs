using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace EasySyntaxTests;

public class PatternMatchingVariableDeclarationHelperTests
{
    [Fact]
    public void SingleVariableDesignation()
    {
        VariableDesignationSyntax designation = SyntaxFactory.SingleVariableDesignation(
            SyntaxFactory.Identifier("x")
        );
        var code = designation.NormalizeWhitespace().ToFullString();
        Assert.Equal("x", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(designation, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(designation, vars));
    }

    [Fact]
    public void ParenthesizedVariableDesignation()
    {
        VariableDesignationSyntax designation = SyntaxFactory.ParenthesizedVariableDesignation(
            SyntaxFactory.SeparatedList(new List<VariableDesignationSyntax>()
            {
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")),
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))
            })
        );
        var code = designation.NormalizeWhitespace().ToFullString();
        Assert.Equal("(x, y)", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(designation, vars));

        vars = new HashSet<string>() { "y" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(designation, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(designation, vars));
    }

    [Fact]
    public void DiscardDesignation()
    {
        VariableDesignationSyntax designation = SyntaxFactory.DiscardDesignation();
        var code = designation.NormalizeWhitespace().ToFullString();
        Assert.Equal("_", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(designation, vars));
    }

    [Fact]
    public void NullTest()
    {
        VariableDesignationSyntax designation = null!;
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(designation, vars));
    }

    [Fact]
    public void DeclarationPattern()
    {
        PatternSyntax pattern = SyntaxFactory.DeclarationPattern(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("int x", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }

    [Fact]
    public void RecursivePattern_WithPositional()
    {
        PatternSyntax pattern = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: SyntaxFactory.PositionalPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>()
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.DeclarationPattern(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
                        )
                        ),
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.DeclarationPattern(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))
                        )
                    )
                })
            ),
            propertyPatternClause: default,
            designation: default
        );

        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("TypeA (int x, int y)", code);

        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "y" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }

    [Fact]
    public void RecursivePattern_WithProperty()
    {
        PatternSyntax pattern = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: default,
            propertyPatternClause: SyntaxFactory.PropertyPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>()
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("PropertyName")),
                        SyntaxFactory.VarPattern(
                            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
                        )
                    ),
                })
            ),
            designation: default
        );
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("TypeA { PropertyName: var x }", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }

    [Fact]
    public void RecursivePattern_WithDesignation()
    {
        PatternSyntax pattern = SyntaxFactory.RecursivePattern(
            SyntaxFactory.IdentifierName("TypeA"),
            positionalPatternClause: default,
            propertyPatternClause: SyntaxFactory.PropertyPatternClause(
                SyntaxFactory.SeparatedList(new List<SubpatternSyntax>()
                {
                    SyntaxFactory.Subpattern(
                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("PropertyName")),
                        SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)))
                    ),
                })
            ),
           designation: SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("TypeA { PropertyName: 42 } x", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }

    [Fact]
    public void VarPattern()
    {
        PatternSyntax pattern = SyntaxFactory.VarPattern(
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
        );
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("var x", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }

    [Fact]
    public void BinaryPattern()
    {
        PatternSyntax pattern = SyntaxFactory.BinaryPattern(
            SyntaxKind.OrPattern,
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42))),
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(99)))
        );
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("42 or 99", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }

    [Fact]
    public void ParenthesizedPattern()
    {
        PatternSyntax pattern = SyntaxFactory.ParenthesizedPattern(
            SyntaxFactory.VarPattern(
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x"))
            )
        );
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("(var x)", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }
    [Fact]
    public void VarParenthesizedPattern()
    {
        VariableDesignationSyntax designation = SyntaxFactory.ParenthesizedVariableDesignation(
            SyntaxFactory.SeparatedList(new List<VariableDesignationSyntax>()
            {
                        SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")),
                        SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("y"))
            })
        );
        PatternSyntax pattern = SyntaxFactory.VarPattern(designation);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("var (x, y)", code);
        ImmutableHashSet<string> vars = new HashSet<string>() { "x" }.ToImmutableHashSet();
        Assert.True(AnyDeclaredVariablesMatch(pattern, vars));

        vars = new HashSet<string>() { "z" }.ToImmutableHashSet();
        Assert.False(AnyDeclaredVariablesMatch(pattern, vars));
    }


    public static bool AnyDeclaredVariablesMatch(PatternSyntax pattern, ImmutableHashSet<string> variableNames)
    {
        return pattern switch
        {
            RecursivePatternSyntax { PositionalPatternClause: var positionalPatternClause, PropertyPatternClause: var propertyPatternClause, Designation: var designation } =>
                (designation is not null && AnyDeclaredVariablesMatch(designation, variableNames))
                    || (propertyPatternClause?.Subpatterns.Any(p => AnyDeclaredVariablesMatch(p.Pattern, variableNames)) ?? false)
                    || (positionalPatternClause?.Subpatterns.Any(p => AnyDeclaredVariablesMatch(p.Pattern, variableNames)) ?? false),
            BinaryPatternSyntax binaryPattern =>
                AnyDeclaredVariablesMatch(binaryPattern.Left, variableNames)
                    || AnyDeclaredVariablesMatch(binaryPattern.Right, variableNames),
            ParenthesizedPatternSyntax parenthesizedPattern => AnyDeclaredVariablesMatch(parenthesizedPattern.Pattern, variableNames),
            DeclarationPatternSyntax { Designation: var variableDesignation } => AnyDeclaredVariablesMatch(variableDesignation, variableNames),
            VarPatternSyntax { Designation: var variableDesignation } => AnyDeclaredVariablesMatch(variableDesignation, variableNames),
            _ => false
        };
    }

    internal static bool AnyDeclaredVariablesMatch(VariableDesignationSyntax designation, ImmutableHashSet<string> variableNames)
    {
        return designation switch
        {
            SingleVariableDesignationSyntax singleVariableDesignation => variableNames.Contains(singleVariableDesignation.Identifier.ValueText),
            ParenthesizedVariableDesignationSyntax parenthesizedVariableDesignation => parenthesizedVariableDesignation.Variables.Any(variable => AnyDeclaredVariablesMatch(variable, variableNames)),
            DiscardDesignationSyntax _ => false,
            _ => false
        };
    }
}
