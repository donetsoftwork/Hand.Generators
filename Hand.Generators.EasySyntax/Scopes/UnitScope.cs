using Hand.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Scopes;

/// <summary>
/// 编译作用域
/// </summary>
/// <param name="usings"></param>
/// <param name="types"></param>
public class UnitScope(HashSet<UsingDirectiveSyntax> usings, List<ITypeGenerator> types)
    : ScopeBase
{
    #region 配置
    /// <summary>
    /// 引用
    /// </summary>
    protected readonly HashSet<UsingDirectiveSyntax> _usings = usings;
    /// <summary>
    /// 类型
    /// </summary>
    protected readonly List<ITypeGenerator> _types = types;
    /// <inheritdoc />
    public override NameSyntax Name 
        => NamespaceScope.Empty;
    #endregion

    /// <inheritdoc />
    public override void Using(UsingDirectiveSyntax directive)
    {
        if (_usings.Contains(directive))
            return;
        _usings.Add(directive);
    }
    /// <inheritdoc />
    public override void Add(ITypeGenerator type)
        => _types.Add(type);
    /// <summary>
    /// 构造语法根
    /// </summary>
    /// <returns></returns>
    public override CompilationUnitSyntax BuildUnit()
        => SyntaxFactory.CompilationUnit(default, [.. _usings], default, BuildMembers(), SyntaxFactory.Token(SyntaxKind.EndOfFileToken));
    /// <summary>
    /// 构造成员
    /// </summary>
    /// <returns></returns>
    public virtual SyntaxList<MemberDeclarationSyntax> BuildMembers()
        => [.. _types.ConvertAll(item => item.BuildType())];

    /// <summary>
    /// 构造编译作用域
    /// </summary>
    /// <returns></returns>
    public static UnitScope Create()
        => new(new(UsingDirectiveComparer.Instance), []);
}
