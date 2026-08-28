using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Members;

/// <summary>
/// 属性定义成员
/// </summary>
/// <param name="memberSymbol"></param>
/// <param name="original"></param>
/// <param name="summary"></param>
public class PropertyDeclarationMember(ITypeSymbolInfo memberSymbol, PropertyDeclarationSyntax original, Func<string> summary)
    : Member(original.Identifier.ValueText, memberSymbol, summary, original.GetSummary)
{
    #region 配置
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.PropertyDeclaration;
    /// <summary>
    /// 原始属性定义
    /// </summary>
    public PropertyDeclarationSyntax Original { get; } = original;
    #endregion
}
