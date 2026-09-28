using Hand.Members;
using Hand.Methods;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand.Naming;

/// <summary>
/// 类成员命名器
/// </summary>
/// <param name="typeName"></param>
/// <param name="original"></param>
public sealed class TypedProvider(string typeName, INamingProvider original)
    : INamingProvider
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="typeName"></param>
    public TypedProvider(string typeName)
        : this(typeName, new KindContainer())
    {
    }
    #region 配置
    private readonly string _typeName = typeName;
    private readonly INamingProvider _original = original;

    /// <summary>
    /// 类型名
    /// </summary>
    public string TypeName 
        => _typeName;
    /// <summary>
    /// 原始命名器
    /// </summary>
    public INamingProvider Original 
        => _original;
    #endregion
    #region INamingProvider
    /// <inheritdoc />
    public bool Contains(string name)
        => _typeName.Equals(name) || _original.Contains(name);
    /// <inheritdoc />
    public void AddKind(string name, MemberKind kind)
        => _original.AddKind(name, kind);
    /// <inheritdoc />
    public bool Contains(MethodSignature method)
        => _typeName.Equals(method.Name) || _original.Contains(method);
    /// <inheritdoc />
    public bool TryDeclareMethod(string name, params ITypeSymbol[] parameterSymbols)
           => !_typeName.Equals(name) && _original.TryDeclareMethod(name, parameterSymbols);
    /// <inheritdoc />
    public bool TryDeclareMethod(MethodSignature method)
        => !_typeName.Equals(method.Name) && _original.TryDeclareMethod(method);
    /// <inheritdoc />
    public bool TryGetMemberKind(string name, out MemberKind kind)
    {
        if (_typeName.Equals(name))
        {
            kind = MemberKind.Unknown;
            return true;
        }
        return _original.TryGetMemberKind(name, out kind);
    }
    #endregion
    #region Create
    /// <summary>
    /// 按成员构造命名器
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedProvider Create(string typeName, params IEnumerable<ISymbol> members)
        => new(typeName, KindContainer.Create(members));
    /// <summary>
    /// 按类型符号构造命名器
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedProvider Create(INamedTypeSymbol symbol)
        => new(symbol.Name, KindContainer.Create(symbol));
    /// <summary>
    /// 仅当前定义成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypedProvider DeclaredOnly(INamedTypeSymbol symbol)
        => new(symbol.Name, KindContainer.DeclaredOnly(symbol));
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
        var @base = KindContainer.Create(SymbolReflection.GeNonPrivateMembersWithBase(baseSymbol));
        return new InheritProvider(original, @base);
    }
    #endregion
}
