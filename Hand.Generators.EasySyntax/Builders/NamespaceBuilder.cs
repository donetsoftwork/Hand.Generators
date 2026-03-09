using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 命名空间语法构造器
/// </summary>
/// <param name="ns"></param>
/// <param name="usings"></param>
/// <param name="type"></param>
/// <param name="members"></param>
public class NamespaceBuilder(BaseNamespaceDeclarationSyntax ns, List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, List<MemberDeclarationSyntax> members)
    : SyntaxGenerator(usings, type,  members)
{
    #region 配置
    private readonly BaseNamespaceDeclarationSyntax _ns = ns;
    /// <summary>
    /// 命名空间
    /// </summary>
    public BaseNamespaceDeclarationSyntax NS 
        => _ns;
    #endregion
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <returns></returns>
    public override CompilationUnitSyntax Build()
        => Build(_ns, _usings, _type, [.. _parameters], [.. _members]);
}
