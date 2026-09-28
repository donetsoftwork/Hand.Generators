using Hand.Converters.Methods;
using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.System;

/// <summary>
/// 字符串转化器
/// </summary>
public class ToStringConverter()
    : MethodConverter(_toString)
{
    #region 配置
    private static readonly SyntaxWrapper<SimpleNameSyntax> _toString = new(SyntaxFactory.IdentifierName("ToString"));
    #endregion

    /// <summary>
    /// 单例
    /// </summary>
    public static readonly ToStringConverter Instance = new();
}
