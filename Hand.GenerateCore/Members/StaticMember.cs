using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Members;

/// <summary>
/// 静态成员
/// </summary>
/// <param name="namespace"></param>
/// <param name="typeName"></param>
/// <param name="memberName"></param>
public class StaticMember(NameSyntax @namespace, string typeName, SimpleNameSyntax memberName)
    : ISyntaxDisplay<NameSyntax>
{
    private readonly NameSyntax _namespace = @namespace;
    private readonly string _typeName = typeName;
    private readonly SimpleNameSyntax _memberName = memberName;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="typeName"></param>
    /// <param name="memberName"></param>
    public StaticMember(NameSyntax @namespace, string typeName, string memberName)
        : this(@namespace, typeName, SyntaxFactory.IdentifierName(memberName))
    {
    }
    /// <inheritdoc />
    public NameSyntax Display(SyntaxGenerator generator)
        => generator.DisplayMember(_typeName, _namespace, _memberName);

    /// <summary>
    /// 构造静态成员
    /// </summary>
    /// <param name="member">静态属性或字段</param>
    /// <returns></returns>
    public static StaticMember Create(ISymbol member)
        => Create(member.ContainingType, member.Name);
    /// <summary>
    /// 构造静态成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static StaticMember Create(INamedTypeSymbol type, string memberName)
        => new(type.ContainingNamespace.ToName(), type.Name, memberName);
}
