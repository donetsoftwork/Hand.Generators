using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 类语法树构造器
/// </summary>
/// <param name="usings">引用</param>
/// <param name="type">类</param>
/// <param name="constructors">构造函数</param>
/// <param name="fields">字段</param>
/// <param name="properties">属性</param>
/// <param name="methods">方法</param>
public partial class SyntaxGenerator(List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
{
    #region 配置
    /// <summary>
    /// 引用
    /// </summary>
    protected readonly List<UsingDirectiveSyntax> _usings = usings;
    /// <summary>
    /// 类型
    /// </summary>
    protected TypeDeclarationSyntax _type = type;
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
    /// 成员
    /// </summary>
    protected readonly List<MemberDeclarationSyntax> _others = [];
    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    /// <summary>
    /// 成员
    /// </summary>
    public IEnumerable<MemberDeclarationSyntax> Members
        => _fields.Concat<MemberDeclarationSyntax>(_properties).Concat(_methods).Concat(_others);
    #endregion
    #region Using
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="usings"></param>
    public void Using(params IReadOnlyCollection<UsingDirectiveSyntax> usings)
    {
        if(usings.Count == 0)
            return;
        var delta = Plus(_usings, usings);
        if (delta.Count == 0)
            return;
        _usings.AddRange(delta);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="names"></param>
    public void Using(params IReadOnlyCollection<string> names)
    {
        if (names.Count == 0)
            return;
        var delta = Plus(_usings, names);
        if (delta.Count == 0)
            return;
        _usings.AddRange(delta);
    }
    /// <summary>
    /// 增加基类
    /// </summary>
    /// <param name="baseType"></param>
    public void AddBaseType(BaseTypeSyntax baseType)
        => _type = (TypeDeclarationSyntax)_type.AddBaseListTypes(baseType);
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="parameter"></param>
    public void AddParameter(ParameterSyntax parameter)
        => _type = _type.AddParameterListParameters(parameter);
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public void Parameter(TypeSyntax type, SyntaxToken name)
        => AddParameter(type.Parameter(name));
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public void Parameter(TypeSyntax type, string name)
        => AddParameter(type.Parameter(name));
    /// <summary>
    /// 应用类型修改
    /// </summary>
    /// <param name="modify"></param>
    public void Apply(Func<TypeDeclarationSyntax, TypeDeclarationSyntax> modify)
        => _type = modify(_type);
    /// <summary>
    /// 添加构造函数
    /// </summary>
    /// <param name="constructor"></param>
    public void AddConstructor(ConstructorDeclarationSyntax constructor)
        => _constructors.Add(constructor);
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="field"></param>
    public void AddField(FieldDeclarationSyntax field)
        => _fields.Add(field);
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="property"></param>
    public void AddProperty(PropertyDeclarationSyntax property)
        => _properties.Add(property);
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    public void AddMethod(MethodDeclarationSyntax method)
        => _methods.Add(method);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="member"></param>
    public void AddOther(MemberDeclarationSyntax member)
        => _others.Add(member);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="members"></param>
    public void AddOthers(params MemberDeclarationSyntax[] members)
        => _others.AddRange(members);
    #endregion
    #region Build
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="usings"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax BuildUnit(List<UsingDirectiveSyntax> usings, MemberDeclarationSyntax root)
    {
        return SyntaxFactory.CompilationUnit()
            .WithUsings(List(usings))
            .AddMembers(root)
            .NormalizeWhitespace();
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    private static TypeDeclarationSyntax CheckMembers(TypeDeclarationSyntax type,  MemberDeclarationSyntax[] members)
    {
        if (members.Length > 0)
        {
            if (type.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
            {
                // 如果简化类型(分号结尾)
                // 增加花括号并去掉分号
                return type.AddMembers(members)
                    .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
            }
            else
            {
                return type.AddMembers(members);
            }
        }
        if(type.Members.Count == 0 && !type.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
        {
            // 如果没有成员且没有分号结尾
            // 增加分号结尾
            return type.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        return type;
    }
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax Build(List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, CheckMembers(type, members));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax Build(BaseNamespaceDeclarationSyntax ns, List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, ns.AddMembers(CheckMembers(type, members)));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CompilationUnitSyntax Build()
        => Build(_usings, _type, [.. _constructors, .. _fields, .. _properties, .. _methods, .. _others]);
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
            return new SyntaxGenerator([], typeNew, [], [], [], []);
        else if(parent is BaseNamespaceDeclarationSyntax ns)
            // 清空成员并注释
            return new NamespaceBuilder(ns.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>()).WithLeadingTrivia(SyntaxFactory.TriviaList()), [], typeNew, [], [], [], []);
        else if (parent is CompilationUnitSyntax cu)
            return new SyntaxGenerator([.. cu.Usings], typeNew, [], [], [], []);
        return new SyntaxGenerator([], typeNew, [], [], [], []); 
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
        => new([], type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(TypeDeclarationSyntax type)
        => new([], type, [], [], [], []);
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
    public static NamespaceBuilder Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(ns, [], type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type)
        => new(ns, [], type, [], [], [], []);
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
    public static NamespaceBuilder Create(string ns, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), [], type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(string ns, TypeDeclarationSyntax type)
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), [], type, [], [], [], []);
    #endregion
}
