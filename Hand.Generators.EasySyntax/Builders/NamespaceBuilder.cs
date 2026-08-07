using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 命名空间语法构造器
/// </summary>
/// <param name="ns"></param>
/// <param name="usings"></param>
/// <param name="type">类</param>
/// <param name="constructors">构造函数</param>
/// <param name="fields">字段</param>
/// <param name="properties">属性</param>
/// <param name="methods">方法</param>
public class NamespaceBuilder(BaseNamespaceDeclarationSyntax ns, List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
    : SyntaxGenerator(usings, type, constructors, fields, properties, methods)
{
    #region 配置
    private BaseNamespaceDeclarationSyntax _ns = ns;
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
        => Build(_ns, _usings, _type, [.. _constructors, .. _fields, .. _properties, .. _methods, .. _others]);
    /// <summary>
    /// 应用命名空间修改
    /// </summary>
    /// <param name="modify"></param>
    public void Apply(Func<BaseNamespaceDeclarationSyntax, BaseNamespaceDeclarationSyntax> modify)
        => _ns = modify(_ns);
}
