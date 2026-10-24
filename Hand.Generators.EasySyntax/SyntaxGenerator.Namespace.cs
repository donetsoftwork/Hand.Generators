//using Hand.Collections;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System;
//using System.Collections.Generic;

//namespace Hand;

///// <summary>
///// 命名空间语法构造器
///// </summary>
///// <param name="ns"></param>
///// <param name="usings"></param>
///// <param name="declaration">类型声明</param>
///// <param name="constructors">构造函数</param>
///// <param name="fields">字段</param>
///// <param name="properties">属性</param>
///// <param name="methods">方法</param>
//public class NamespaceGenerator(BaseNamespaceDeclarationSyntax ns, HashSet<UsingDirectiveSyntax> usings, TypeDeclarationSyntax declaration, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
//    : SyntaxGenerator(usings, declaration, constructors, fields, properties, methods)
//{
//    #region 配置
//    private readonly BaseNamespaceDeclarationSyntax _ns = ns;
//    private readonly NameSyntax _namespace = ns.Name;
//    private readonly string _namespaceName = ns.Name.ToFullString();
//    private readonly List<TypeDeclarationSyntax> _otherTypes = [];
//    /// <summary>
//    /// 命名空间
//    /// </summary>
//    public BaseNamespaceDeclarationSyntax NS
//        => _ns;
//    ///// <summary>
//    ///// 其他类型
//    ///// </summary>
//    //public IEnumerable<TypeDeclarationSyntax> OtherTypes
//    //    => _otherTypes;
//    #endregion
//    /// <summary>
//    /// 尝试获取命名空间
//    /// </summary>
//    /// <param name="typeName"></param>
//    /// <param name="namespace"></param>
//    /// <returns></returns>
//    protected override bool TryGetNamespace(string typeName, out string? @namespace)
//    {
//        if (string.Equals(_typeName, typeName, StringComparison.Ordinal))
//        {
//            @namespace = _namespaceName;
//            return true;
//        }
//        return base.TryGetNamespace(typeName, out @namespace);
//    }
//    /// <summary>
//    /// 添加Using
//    /// </summary>
//    /// <param name="directive"></param>
//    public override void Using(UsingDirectiveSyntax directive)
//    {
//        var alias = directive.Alias;
//        if (alias is null && TypeComparer.Equals(directive.NamespaceOrType, _namespace))
//            return;
//        base.Using(directive);
//    }
//    /// <summary>
//    /// 添加Using
//    /// </summary>
//    /// <param name="namespace"></param>
//    public override void Using(string @namespace)
//    {
//        if (string.Equals(_namespaceName, @namespace, StringComparison.Ordinal))
//            return;
//        var directive = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace));
//        base.Using(directive);
//    }
//    /// <summary>
//    /// 构造语法树
//    /// </summary>
//    /// <returns></returns>
//    public override CompilationUnitSyntax BuildUnit()
//        => BuildUnit(_ns, _usings, _declaration, [.. _constructors, .. _fields, .. _properties, .. _methods, .. _others], _otherTypes);
//    ///// <summary>
//    ///// 应用命名空间修改
//    ///// </summary>
//    ///// <param name="modify"></param>
//    //public void Apply(Func<BaseNamespaceDeclarationSyntax, BaseNamespaceDeclarationSyntax> modify)
//    //{
//    //    _ns = modify(_ns);
//    //    _namespaceName = _ns.ToFullString();
//    //}
//    ///// <summary>
//    ///// 添加其他类型
//    ///// </summary>
//    ///// <param name="otherType"></param>
//    //public void AddOtherType(TypeDeclarationSyntax otherType)
//    //    => _otherTypes.Add(otherType);
//}