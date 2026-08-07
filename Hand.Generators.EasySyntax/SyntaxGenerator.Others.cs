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
    /// <summary>
    /// 参数列表
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterListSyntax ParameterList(params IEnumerable<ParameterSyntax> parameters)
        => SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters));
    #region List
    /// <summary>
    /// 集合转化
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static SyntaxList<TItem> List<TItem>(TItem[] items)
        where TItem : SyntaxNode
    {
        return items.Length switch
        {
            0 => default,
            1 => new SyntaxList<TItem>(items[0]),
            _ => [.. items],
        };
    }
    /// <summary>
    /// 集合转化
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static SyntaxList<TItem> List<TItem>(IList<TItem> items)
        where TItem : SyntaxNode
    {
        return items.Count switch
        {
            0 => default,
            1 => new SyntaxList<TItem>(items[0]),
            _ => [.. items],
        };
    }
    #endregion
    #region ArgumentList
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<ExpressionSyntax> arguments)
        => ArgumentList(arguments.Select(SyntaxFactory.Argument));
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<SyntaxToken> arguments)
        => ArgumentList(arguments.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<string> arguments)
        => ArgumentList(arguments.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    #endregion
    #region Plus
    /// <summary>
    /// 追加引用
    /// </summary>
    /// <param name="list0">原引用</param>
    /// <param name="delta">引用增量</param>
    /// <returns></returns>
    public static IReadOnlyCollection<UsingDirectiveSyntax> Plus(IEnumerable<UsingDirectiveSyntax> list0, params IReadOnlyCollection<UsingDirectiveSyntax> delta)
    {
        var keys = new HashSet<string>(list0
            .Select(item => item.ToFullString())
            .Distinct());
        var list = new List<UsingDirectiveSyntax>(delta.Count);
        foreach (var item in delta)
        {
            var key = item.ToFullString();
            if (keys.Contains(key))
                continue;
            keys.Add(key);
            list.Add(item);
        }
        return list;
    }
    /// <summary>
    /// 追加引用
    /// </summary>
    /// <param name="list0">原引用</param>
    /// <param name="delta">引用增量</param>
    /// <returns></returns>
    public static IReadOnlyCollection<UsingDirectiveSyntax> Plus(IEnumerable<UsingDirectiveSyntax> list0, params IReadOnlyCollection<string> delta)
    {
        var keys = new HashSet<string>(list0
            .Select(item => item.ToFullString())
            .Distinct());
        var list = new List<UsingDirectiveSyntax>(delta.Count);
        foreach (var item in delta)
        {
            if (keys.Contains(item))
                continue;
            keys.Add(item);
            list.Add(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(item)));
        }
        return list;
    }
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
