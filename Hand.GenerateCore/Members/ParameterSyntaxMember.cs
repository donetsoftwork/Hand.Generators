using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Members;

/// <summary>
/// 参数定义成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
/// <param name="summary"></param>
public class ParameterSyntaxMember(string name, ParameterSyntax original, ITypeSymbolInfo symbolInfo, Func<string> summary)
    : Member(name, symbolInfo, summary, original.GetSummary)
{
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.ParameterDeclaration;
    /// <summary>
    /// 原始参数
    /// </summary>
    public ParameterSyntax Original { get; } = original;
}
