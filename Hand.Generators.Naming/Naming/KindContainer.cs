using Hand.Members;
using Hand.Methods;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand.Naming;

/// <summary>
/// 分类成员命名器
/// </summary>
/// <param name="members"></param>
public sealed class KindContainer(IDictionary<string, MemberKind> members)
    : INamingProvider
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public KindContainer()
        : this(new Dictionary<string, MemberKind>())
    {
    }
    #region 配置
    /// <summary>
    /// 成员分类(用于排重)
    /// </summary>
    private readonly IDictionary<string, MemberKind> _members = members;
    #endregion
    #region INamingProvider
    /// <inheritdoc />
    public bool Contains(string name)
        => _members.ContainsKey(name);
    /// <inheritdoc />
    public bool TryGetMemberKind(string name, out MemberKind kind)
        => _members.TryGetValue(name, out kind);
    /// <inheritdoc />
    public bool Contains(MethodSignature method)
        => _members.ContainsKey(method.Name);
    /// <inheritdoc />
    public bool TryDeclareMethod(string name, params ITypeSymbol[] parameterSymbols)
    {
        if (_members.ContainsKey(name))
            return false;
        AddKind(name, MemberKind.Method);
        return true;
    }
    /// <inheritdoc />
    public bool TryDeclareMethod(MethodSignature method)
    {
        var name = method.Name;
        if (_members.ContainsKey(name))
            return false;
        AddKind(name, MemberKind.Method);
        return true;
    }
    /// <summary>
    /// 添加成员类型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="kind"></param>
    public void AddKind(string name, MemberKind kind)
        => _members[name] = kind;
    #endregion
    #region AddMethod
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    public void AddMethod(MethodDeclarationSyntax method)
    {
        // 忽略显示实现接口的方法
        if (method.ExplicitInterfaceSpecifier is null)
            AddKind(method.Identifier.ValueText, MemberKind.MethodDeclaration);
    }
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="method"></param>
    public void AddMethod(string name, MethodDeclarationSyntax method)
    {
        if (method.ExplicitInterfaceSpecifier is null)
            AddKind(name, MemberKind.MethodDeclaration);
    }
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="name"></param>
    public void AddMethod(string name)
        => AddKind(name, MemberKind.Method);
    #endregion
    /// <summary>
    /// 转化为字典
    /// </summary>
    /// <param name="members"></param>
    /// <returns></returns>
    public static IDictionary<string, MemberKind> ToDictionary(IEnumerable<ISymbol> members)
    {
        var dictionary = new Dictionary<string, MemberKind>();
        foreach (var item in members)
        {
            switch (item.Kind)
            {
                case SymbolKind.Field:
                    dictionary[item.Name] = MemberKind.Field;
                    break;
                case SymbolKind.Property:
                    dictionary[item.Name] = MemberKind.Property;
                    break;
                case SymbolKind.Method:
                    dictionary[item.Name] = MemberKind.Method;
                    break;
                default:
                    break;
            }
        }
        return dictionary;
    }
    #region Create
    /// <summary>
    /// 按成员构造命名器
    /// </summary>
    /// <param name="members"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KindContainer Create(params IEnumerable<ISymbol> members)
        => new(ToDictionary(members));
    /// <summary>
    /// 按类型符号构造命名器
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KindContainer Create(INamedTypeSymbol symbol)
        => Create(SymbolReflection.GetMembersWithBase(symbol).Where(static item => !item.IsImplicitlyDeclared));
    /// <summary>
    /// 仅当前定义成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static KindContainer DeclaredOnly(INamedTypeSymbol symbol)
        => Create(symbol.GetMembers().Where(static item => !item.IsImplicitlyDeclared));
    /// <summary>
    /// 继承基类成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static InheritProvider Inherit(INamedTypeSymbol symbol)
    {
        var baseSymbol = symbol.BaseType;
        if (baseSymbol is null)
        {
            // 只有object没有基类,该逻辑应该不会命中
            var original = DeclaredOnly(symbol);
            return new InheritProvider(original, new KindContainer());
        }
        return Inherit(symbol, baseSymbol);
    }
    /// <summary>
    /// 继承基类成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="baseSymbol"></param>
    /// <returns></returns>
    public static InheritProvider Inherit(INamedTypeSymbol symbol, INamedTypeSymbol baseSymbol)
    {
        var original = DeclaredOnly(symbol);
        var @base = Create(SymbolReflection.GeNonPrivateMembersWithBase(baseSymbol));
        return new InheritProvider(original, @base);
    }
    #endregion
}
