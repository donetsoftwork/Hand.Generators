using Hand.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Scopes;

/// <summary>
/// 命名空间作用域
/// </summary>
/// <param name="ns"></param>
/// <param name="usings"></param>
/// <param name="types"></param>
public class NamespaceScope(BaseNamespaceDeclarationSyntax ns, HashSet<UsingDirectiveSyntax> usings, List<ITypeGenerator> types)
    : UnitScope(usings, types)
{
    #region namespace
    /// <summary>
    /// 空
    /// </summary>
    public static readonly IdentifierNameSyntax Empty = SyntaxFactory.IdentifierName(string.Empty);
    /// <summary>
    /// System
    /// </summary>
    public static readonly IdentifierNameSyntax System = SyntaxFactory.IdentifierName("System");
    /// <summary>
    /// System.Collections
    /// </summary>
    public static readonly QualifiedNameSyntax Collections = SyntaxFactory.IdentifierName("Collections").Qualify(System);
    /// <summary>
    /// System.Collections.Generic
    /// </summary>
    public static readonly QualifiedNameSyntax GenericCollections = SyntaxFactory.IdentifierName("Generic").Qualify(Collections);
    /// <summary>
    /// Hand
    /// </summary>
    public static readonly IdentifierNameSyntax Hand = SyntaxFactory.IdentifierName("Hand");
    #endregion
    #region 配置
    private readonly BaseNamespaceDeclarationSyntax _ns = ns;
    private readonly NameSyntax _name = ns.Name;
    /// <summary>
    /// 命名
    /// </summary>
    public override NameSyntax Name 
        => _name;
    #endregion

    /// <inheritdoc />
    public override SyntaxList<MemberDeclarationSyntax> BuildMembers()
        => [_ns.WithMembers(base.BuildMembers())];
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="directive"></param>
    public override void Using(UsingDirectiveSyntax directive)
    {
        var alias = directive.Alias;
        if (alias is null && TypeComparer.Equal(directive.NamespaceOrType, _name))
            return;
        base.Using(directive);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="namespace"></param>
    public override void Using(NameSyntax @namespace)
    {
        if (TypeComparer.Equal(_name, @namespace))
            return;
        base.Using(@namespace);
    }
    /// <summary>
    /// 构造命名空间作用域
    /// </summary>
    /// <returns></returns>
    public static NamespaceScope Create(BaseNamespaceDeclarationSyntax ns)
        => new(ns, new HashSet<UsingDirectiveSyntax>(UsingDirectiveComparer.Instance), []);
}
