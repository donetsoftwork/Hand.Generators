using Hand.Collections;
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
/// <param name="usings">引用</param>
/// <param name="type">类</param>
/// <param name="constructors">构造函数</param>
/// <param name="fields">字段</param>
/// <param name="properties">属性</param>
/// <param name="methods">方法</param>
public partial class SyntaxGenerator(HashSet<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
{
    #region 配置
    /// <summary>
    /// 引用
    /// </summary>
    protected readonly HashSet<UsingDirectiveSyntax> _usings = usings;
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
    /// 类名
    /// </summary>
    private readonly Dictionary<string, string> _metadataNames = [];
    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    #endregion
    #region Using
    /// <summary>
    /// 展示类名
    /// 支持泛型定义不支持闭合泛型
    /// 闭合泛型请使用Hand.GenerateCore的扩展方法TypeSyntax Display(this SyntaxGenerator generator, GenericTypeInfo info)
    /// 暂不支持内部类
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    public TypeSyntax Display(INamedTypeSymbol symbol, bool isNullable)
        => Display(symbol).Nullable(isNullable);
    /// <summary>
    /// 展示类名
    /// 支持泛型定义不支持闭合泛型
    /// 闭合泛型请使用Hand.GenerateCore的扩展方法TypeSyntax Display(this SyntaxGenerator generator, GenericTypeInfo info)
    /// 暂不支持内部类
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public TypeSyntax Display(INamedTypeSymbol symbol)
    {
        var containingNamespace = symbol.ContainingNamespace.ToDisplayString();
        SimpleNameSyntax type = GetTypeName(symbol);
        var metadataName = symbol.MetadataName;
        if (TryGetNamespace(metadataName, out var @namespace))
        {
            if (!string.Equals(containingNamespace, @namespace))
                return type.Qualify(containingNamespace);
        }
        else
        {
            _metadataNames[metadataName] = containingNamespace;
            Using(containingNamespace);
        }
        return type;
    }
    /// <summary>
    /// 获取类型名(支持泛型定义不支持闭合泛型)
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static SimpleNameSyntax GetTypeName(INamedTypeSymbol symbol)
    {
        if (symbol.IsGenericType)
        {
            var parameters = symbol.TypeParameters;
            var count = parameters.Length;
            var arguments = new TypeSyntax[count];
            for (var i = 0; i < count; i++)
                arguments[i] = SyntaxFactory.IdentifierName(parameters[i].Name);
            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(symbol.Name), SyntaxFactory.TypeArgumentList([.. arguments]));
        }
        return SyntaxFactory.IdentifierName(symbol.Name);
    }
    /// <summary>
    /// 展示类名
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <returns></returns>
    public TypeSyntax Display(string typeName, string @namespace)
    {
        TypeSyntax type;
        if (TryGetNamespace(typeName, out var @namespace0))
        {
            if (string.Equals(@namespace0, @namespace))
                type = SyntaxFactory.IdentifierName(typeName);
            else
                type = SyntaxFactory.IdentifierName(typeName).Qualify(@namespace);
        }
        else
        {
            type = SyntaxFactory.IdentifierName(typeName);
            _metadataNames[typeName] = @namespace;
            Using(@namespace);
        }
        return type;
    }
    /// <summary>
    /// 展示泛型
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public TypeSyntax GenericDisplay(string typeName, string @namespace, params TypeSyntax[] arguments)
    {
        TypeSyntax type;
        var metadataName = $"{typeName}`{arguments.Length}";
        if (TryGetNamespace(metadataName, out var @namespace0))
        {
            if (string.Equals(@namespace0, @namespace))
                type = Generic(typeName, arguments);
            else
                type = Generic(typeName, arguments).Qualify(@namespace);
        }
        else
        {
            type = Generic(typeName, arguments);
            _metadataNames[metadataName] = @namespace;
            Using(@namespace);
        }
        return type;
    }
    /// <summary>
    /// 尝试获取命名空间
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <returns></returns>
    protected virtual bool TryGetNamespace(string typeName, out string? @namespace)
        => _metadataNames.TryGetValue(typeName, out @namespace);
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="directive"></param>
    public virtual void Using(UsingDirectiveSyntax directive)
    {
        if(_usings.Contains(directive))
            return;
        _usings.Add(directive);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="namespace"></param>
    public virtual void Using(string @namespace)
    {
        var directive = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace));
        Using(directive);
    }
    /// <summary>
    /// 添加using System
    /// </summary>
    public void UsingSystem()
        => Using(SystemDirective);
    /// <summary>
    /// 添加using System.Collections.Generic
    /// </summary>
    public void UsingCollections()
        => Using(CollectionDirective);
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
    public static CompilationUnitSyntax BuildUnit(HashSet<UsingDirectiveSyntax> usings, MemberDeclarationSyntax root)
    {
        return SyntaxFactory.CompilationUnit()
            .WithUsings([.. usings])
            .AddMembers(root);
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
    public static CompilationUnitSyntax Build(HashSet<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, CheckMembers(type, members));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <param name="otherTypes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax Build(BaseNamespaceDeclarationSyntax ns, HashSet<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, MemberDeclarationSyntax[] members, IEnumerable<TypeDeclarationSyntax> otherTypes)
        => BuildUnit(usings, ns.AddMembers([CheckMembers(type, members), .. otherTypes]));
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
            return new SyntaxGenerator(new(UsingDirectiveComparer.Instance), typeNew, [], [], [], []);
        else if(parent is BaseNamespaceDeclarationSyntax ns)
            // 清空成员与注释
            return new NamespaceBuilder(ns.WithUsings([]).WithMembers([]).WithLeadingTrivia(), new(UsingDirectiveComparer.Instance), typeNew.WithLeadingTrivia(), [], [], [], []);
        return new SyntaxGenerator(new(UsingDirectiveComparer.Instance), typeNew, [], [], [], []); 
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
        => new(new(UsingDirectiveComparer.Instance), type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(TypeDeclarationSyntax type)
        => new(new(UsingDirectiveComparer.Instance), type, [], [], [], []);
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
        => new(ns, new(UsingDirectiveComparer.Instance), type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type)
        => new(ns, new(UsingDirectiveComparer.Instance), type, [], [], [], []);
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
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), new(UsingDirectiveComparer.Instance), type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(string ns, TypeDeclarationSyntax type)
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), new(UsingDirectiveComparer.Instance), type, [], [], [], []);
    #endregion
}
