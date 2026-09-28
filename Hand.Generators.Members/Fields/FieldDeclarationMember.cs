using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Fields;

/// <summary>
/// 字段定义成员
/// </summary>
/// <param name="name"></param>
/// <param name="memberSymbol"></param>
/// <param name="original"></param>
/// <param name="isPublic"></param>
/// <param name="summary"></param>
public class FieldDeclarationMember(string name, ITypeSymbolInfo memberSymbol, FieldDeclarationSyntax original, bool isPublic, string summary)
    : SyntaxMember(name, original, memberSymbol, summary), IMemberInfo
{
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.FieldDeclaration;
    /// <summary>
    /// 原始字段定义
    /// </summary>
    public new FieldDeclarationSyntax Original { get; } = original;
    /// <inheritdoc />
    public override bool IsPublic { get; } = isPublic;
}
