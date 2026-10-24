using Hand.Scopes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 类语法树构造器
/// </summary>
/// <param name="container">父容器</param>
/// <param name="declaration">类型声明</param>
/// <param name="constructors">构造函数</param>
/// <param name="fields">字段</param>
/// <param name="properties">属性</param>
/// <param name="methods">方法</param>
public partial class SyntaxGenerator(ISyntaxScope container, TypeDeclarationSyntax declaration, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
    : TypeGeneratorBase(container, declaration.Identifier.ValueText), ITypeGenerator
{
    #region 配置
    /// <summary>
    /// 类型
    /// </summary>
    protected TypeDeclarationSyntax _declaration = declaration;
    /// <summary>
    /// 构造函数
    /// </summary>
    protected readonly List<ConstructorDeclarationSyntax> _constructors = constructors;
    /// <summary>
    /// 字段
    /// </summary>
    protected readonly List<FieldDeclarationSyntax> _fields = fields;
    /// <summary>
    /// 属性
    /// </summary>
    protected readonly List<PropertyDeclarationSyntax> _properties = properties;
    /// <summary>
    /// 方法
    /// </summary>
    protected readonly List<MethodDeclarationSyntax> _methods = methods;
    /// <summary>
    /// 基类
    /// </summary>
    private readonly List<BaseTypeSyntax> _baseTypes = [];
    /// <summary>
    /// 主构造函数参数
    /// </summary>
    private readonly List<ParameterSyntax> _listParameters = [];
    /// <summary>
    /// 成员
    /// </summary>
    protected readonly List<MemberDeclarationSyntax> _others = [];
    /// <summary>
    /// 类型声明
    /// </summary>
    public TypeDeclarationSyntax Declaration
        => _declaration;
    #endregion
    #region ITypeGenerator
    /// <summary>
    /// 增加基类
    /// </summary>
    /// <param name="baseType"></param>
    public override void AddBaseType(BaseTypeSyntax baseType)
        => _declaration = (TypeDeclarationSyntax)_declaration.AddBaseListTypes(baseType);
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="parameter"></param>
    public override void AddListParameter(ParameterSyntax parameter)
        => _declaration = _declaration.AddParameterListParameters(parameter);
    /// <summary>
    /// 应用类型修改
    /// </summary>
    /// <param name="modify"></param>
    public override void Apply(Func<TypeDeclarationSyntax, TypeDeclarationSyntax> modify)
        => _declaration = modify(_declaration);
    /// <summary>
    /// 添加构造函数
    /// </summary>
    /// <param name="constructor"></param>
    public override void AddConstructor(ConstructorDeclarationSyntax constructor)
        => _constructors.Add(constructor);
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="field"></param>
    public override void AddField(FieldDeclarationSyntax field)
        => _fields.Add(field);
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="property"></param>
    public override void AddProperty(PropertyDeclarationSyntax property)
        => _properties.Add(property);
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    public override void AddMethod(MethodDeclarationSyntax method)
        => _methods.Add(method);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="members"></param>
    public override void AddOthers(params MemberDeclarationSyntax[] members)
        => _others.AddRange(members);
    /// <summary>
    /// 构造类型
    /// </summary>
    /// <returns></returns>
    public override TypeDeclarationSyntax BuildType()
        => CheckMembers(CheckListParameters(CheckBaseType(_declaration, _baseTypes), _listParameters), [.. _constructors, .. _fields, .. _properties, .. _methods, .. _others, .. _inners.ConvertAll(item => item.BuildType())]);
    #endregion
    #region Build
    /// <summary>
    /// 检查基类
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="baseTypes"></param>
    /// <returns></returns>
    private static TypeDeclarationSyntax CheckBaseType(TypeDeclarationSyntax declaration, List<BaseTypeSyntax> baseTypes)
        => baseTypes.Count == 0 ? declaration : (TypeDeclarationSyntax)declaration.AddBaseListTypes([.. baseTypes]);
    /// <summary>
    /// 检查主构造函数参数
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="listParameters"></param>
    /// <returns></returns>
    private static TypeDeclarationSyntax CheckListParameters(TypeDeclarationSyntax declaration, List<ParameterSyntax> listParameters)
        => listParameters.Count == 0 ? declaration : declaration.AddParameterListParameters([.. listParameters]);
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    private static TypeDeclarationSyntax CheckMembers(TypeDeclarationSyntax declaration, MemberDeclarationSyntax[] members)
    {
        if (members.Length > 0)
        {
            if (declaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
            {
                // 如果简化类型(分号结尾)
                // 增加花括号并去掉分号
                return declaration.AddMembers(members)
                    .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
            }
            else
            {
                return declaration.AddMembers(members);
            }
        }
        if (declaration.Members.Count == 0 && !declaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
        {
            // 如果没有成员且没有分号结尾
            // 增加分号结尾
            return declaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        return declaration;
    }
    #endregion
    /// <summary>
    /// 复制类生成构造器
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SyntaxGenerator Clone(TypeDeclarationSyntax type)
    {
        var typeNew = SyntaxFactory.TypeDeclaration(
            type.Kind(),
            default,
            new SyntaxTokenList(GenerateServices._partial),
            type.Keyword,
            type.Identifier,
            typeParameterList: type.TypeParameterList,
            baseList: null,
            type.ConstraintClauses,
            type.OpenBraceToken,
            default,
            type.CloseBraceToken,
            type.SemicolonToken);

        var parent = type.Parent;
        if (parent is null)
            return new SyntaxGenerator(UnitScope.Create(), typeNew, [], [], [], []);
        else if(parent is BaseNamespaceDeclarationSyntax ns)
            // 清空成员与注释
            return new SyntaxGenerator(NamespaceScope.Create(ns.WithUsings([]).WithMembers([]).WithLeadingTrivia()), typeNew, [], [], [], []);
        return new SyntaxGenerator(UnitScope.Create(), typeNew, [], [], [], []); 
    }
    #region Create
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type">类</param>
    /// <param name="constructors">构造函数</param>
    /// <param name="fields">字段</param>
    /// <param name="properties">属性</param>
    /// <param name="methods">方法</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(UnitScope.Create(), type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(TypeDeclarationSyntax type)
        => new(UnitScope.Create(), type, [], [], [], []);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <param name="constructors">构造函数</param>
    /// <param name="fields">字段</param>
    /// <param name="properties">属性</param>
    /// <param name="methods">方法</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(NamespaceScope.Create(ns), type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type)
        => new(NamespaceScope.Create(ns), type, [], [], [], []);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <param name="constructors">构造函数</param>
    /// <param name="fields">字段</param>
    /// <param name="properties">属性</param>
    /// <param name="methods">方法</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(NameSyntax ns, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(NamespaceScope.Create(SyntaxFactory.NamespaceDeclaration(ns)), type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(NameSyntax ns, TypeDeclarationSyntax type)
        => new(NamespaceScope.Create(SyntaxFactory.NamespaceDeclaration(ns)), type, [], [], [], []);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="container"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(ISyntaxScope container, TypeDeclarationSyntax type)
        => new(container, type, [], [], [], []);
    #endregion
}
