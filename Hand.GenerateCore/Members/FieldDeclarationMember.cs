using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Members;

/// <summary>
/// 字段定义成员
/// </summary>
/// <param name="name"></param>
/// <param name="memberSymbol"></param>
/// <param name="original"></param>
/// <param name="summary"></param>
public class FieldDeclarationMember(string name, ITypeSymbolInfo memberSymbol, FieldDeclarationSyntax original, Func<string> summary)
    : Member(name, memberSymbol, summary, original.GetSummary)
{
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.FieldDeclaration;
    /// <summary>
    /// 原始字段定义
    /// </summary>
    public FieldDeclarationSyntax Original { get; } = original;
}
