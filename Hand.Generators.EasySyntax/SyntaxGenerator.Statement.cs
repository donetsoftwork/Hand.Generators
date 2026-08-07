using Hand.Builders;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 语句
/// </summary>
public partial class SyntaxGenerator
{
    /// <summary>
    /// try
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TryBuilder Try()
        => new();
    /// <summary>
    /// Scope
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScopeBuilder Scope()
        => new([]);
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(string message)
        => SyntaxFactory.IdentifierName("Exception")
        .Throw([Literal(message)]);
}
