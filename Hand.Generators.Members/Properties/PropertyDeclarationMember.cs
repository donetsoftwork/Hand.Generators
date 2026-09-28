using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Properties;

/// <summary>
/// 属性定义成员
/// </summary>
/// <param name="memberSymbol"></param>
/// <param name="original"></param>
/// <param name="isPublic"></param>
/// <param name="summary"></param>
public class PropertyDeclarationMember(ITypeSymbolInfo memberSymbol, PropertyDeclarationSyntax original, bool isPublic, string summary)
    : SyntaxMember(original.Identifier.ValueText, original, memberSymbol, summary), IMemberInfo
{
    #region 配置
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.PropertyDeclaration;
    /// <summary>
    /// 原始属性定义
    /// </summary>
    public new PropertyDeclarationSyntax Original { get; } = original;
    /// <inheritdoc />
    public override bool IsPublic { get; } = isPublic;
    #endregion
}
