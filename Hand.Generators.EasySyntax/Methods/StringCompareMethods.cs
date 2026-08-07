using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Methods;

/// <summary>
/// string.Equals
/// </summary>
public static class StringCompareMethods
{
    /// <summary>
    /// System.StringComparison
    /// </summary>
    public static readonly IdentifierNameSyntax StringComparison = SyntaxFactory.IdentifierName("System.StringComparison");
    /// <summary>
    /// System.StringComparison.OrdinalIgnoreCase
    /// </summary>
    public static readonly ExpressionSyntax OrdinalIgnoreCase = StringComparison.Access("OrdinalIgnoreCase");
    #region Equals
    /// <summary>
    /// Equals
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ExpressionSyntax Equals(ExpressionSyntax @this, ExpressionSyntax value)
        => SyntaxGenerator.StringType.Access(nameof(string.Equals))
        .Invocation([@this, value]);
    /// <summary>
    /// Equals
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static ExpressionSyntax Equals(ExpressionSyntax @this, ExpressionSyntax value, ExpressionSyntax comparison)
        => SyntaxGenerator.StringType.Access(nameof(string.Equals))
        .Invocation([@this, value, comparison]);
    #endregion
    #region StartsWith
    /// <summary>
    /// StartsWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ExpressionSyntax StartsWith(ExpressionSyntax @this, ExpressionSyntax value)
        => @this.Access(nameof(string.StartsWith))
        .Invocation([value]);
    /// <summary>
    /// StartsWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static ExpressionSyntax StartsWith(ExpressionSyntax @this, ExpressionSyntax value, ExpressionSyntax comparison)
        => @this.Access(nameof(string.StartsWith))
        .Invocation([value, comparison]);
    #endregion
    #region EndsWith
    /// <summary>
    /// EndsWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ExpressionSyntax EndsWith(ExpressionSyntax @this, ExpressionSyntax value)
        => @this.Access(nameof(string.EndsWith))
        .Invocation([value]);
    /// <summary>
    /// EndsWith
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static ExpressionSyntax EndsWith(ExpressionSyntax @this, ExpressionSyntax value, ExpressionSyntax comparison)
        => @this.Access(nameof(string.EndsWith))
        .Invocation([value, comparison]);
    #endregion
    /// <summary>
    /// IndexOf
    /// </summary>
    /// <param name="this"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax IndexOf(ExpressionSyntax @this, params IEnumerable<ExpressionSyntax> arguments)
        => @this.Access(nameof(string.IndexOf))
        .Invocation(arguments);
    /// <summary>
    /// LastIndexOf
    /// </summary>
    /// <param name="this"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax LastIndexOf(ExpressionSyntax @this, params IEnumerable<ExpressionSyntax> arguments)
        => @this.Access(nameof(string.LastIndexOf))
        .Invocation(arguments);
    /// <summary>
    /// IsNullOrWhiteSpace
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static ExpressionSyntax IsNullOrWhiteSpace(ExpressionSyntax @this)
        => SyntaxGenerator.StringType.Access(nameof(string.IsNullOrWhiteSpace)).Invocation([@this]);
}
