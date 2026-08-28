using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 其他静态方法
/// </summary>
public partial class SyntaxGenerator
{
    #region ArgumentList
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<ExpressionSyntax> arguments)
        => SyntaxFactory.ArgumentList([.. arguments.Select(SyntaxFactory.Argument)]);
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<SyntaxToken> arguments)
        => SyntaxFactory.ArgumentList([.. arguments.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name)))]);
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<string> arguments)
        => SyntaxFactory.ArgumentList([.. arguments.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name)))]);
    #endregion
    #region FrameworkMajorVersion
    private static readonly Lazy<int> _lazyFrameworkMajorVersion = new(GetFrameworkMajorVersion, true);
    /// <summary>
    /// .net主版本
    /// </summary>
    public static int FrameworkMajorVersion
        => _lazyFrameworkMajorVersion.Value;
    /// <summary>
    /// 获取.net主版本
    /// </summary>
    /// <returns></returns>
    private static int GetFrameworkMajorVersion()
    {
        var attribute = typeof(object).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute is null)
            return 0;
        var versionString = attribute.InformationalVersion;
        int index = versionString.IndexOf('.');
#if NET7_0_OR_GREATER
        if (index >= 0 && int.TryParse(versionString.AsSpan(0, index), out var majorVersion))
#else
        if (index >= 0 && int.TryParse(versionString.Substring(0, index), out var majorVersion))
#endif
            return majorVersion;
        return 0;
    }
    #endregion
}
