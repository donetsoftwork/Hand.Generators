using Hand.Collections.Grouping;
using Hand.Members;
using Hand.Methods;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand.Naming;

/// <summary>
/// 方法容器
/// </summary>
/// <param name="methods"></param>
public sealed class MethodContainer(GroupList<MethodSignature> methods)
    : INamingProvider
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="comparer"></param>
    public MethodContainer(IEqualityComparer<string> comparer)
        : this(new GroupList<MethodSignature>(comparer))
    {
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    public MethodContainer()
        : this(new GroupList<MethodSignature>())
    {
    }
    /// <summary>
    /// 方法
    /// </summary>
    private readonly GroupList<MethodSignature> _methods = methods;
    /// <summary>
    /// 构造函数名
    /// </summary>
    public const string ConstructorName = ".ctor";

    #region INamingProvider
    /// <inheritdoc />
    public bool Contains(string name)
        => _methods.ContainsKey(name);
    /// <inheritdoc />
    public bool TryGetMemberKind(string name, out MemberKind kind)
    {
        var first = _methods.GetValues(name).FirstOrDefault();
        if (first is null)
        {
            kind = MemberKind.Unknown;
            return false;
        }
        kind = ConstructorName.Equals(name) ? MemberKind.Constructor : MemberKind.Method;
        return true;
    }
    /// <inheritdoc />
    public bool Contains(MethodSignature method)
        => MethodContains(method.Name, [.. method.ParameterSymbols]);
    /// <inheritdoc />
    public bool TryDeclareMethod(MethodSignature method)
    {
        if (MethodContains(method.Name, [.. method.ParameterSymbols]))
            return false;
        Add(method);
        return true;
    }
    /// <inheritdoc />
    public bool TryDeclareMethod(string name, params ITypeSymbol[] parameterSymbols)
    {
        if (MethodContains(name, parameterSymbols))
            return false;
        Add(new MethodSignature(name, parameterSymbols));
        return true;
    }
    /// <inheritdoc />
    void INamingProvider.AddKind(string name, MemberKind kind) { }
    #endregion
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    public void Add(MethodSignature method)
        => _methods.Add(method.Name, method);
    /// <summary>
    /// 验证方法是否已经存在
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    public bool MethodContains(string methodName, params ITypeSymbol[] parameterSymbols)
    {
        var list = _methods.GetValues(methodName);
        var parameterCount = parameterSymbols.Length;
        return list.Any(method => method.VerifyParameter(parameterCount, parameterSymbols));
    }
    /// <summary>
    /// 验证构造函数是否已经存在
    /// </summary>
    /// <returns></returns>
    public bool HasConstructor()
        => _methods.ContainsKey(ConstructorName);
    /// <summary>
    /// 验证构造函数是否已经存在
    /// </summary>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    public bool ConstructorContains(params ITypeSymbol[] parameterSymbols)
        => MethodContains(ConstructorName, parameterSymbols);
    /// <summary>
    /// 转化为分组列表
    /// </summary>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static GroupList<MethodSignature> ToGroupList(params IEnumerable<IMethodSymbol> methods)
    {
        var list = new GroupList<MethodSignature>();
        foreach (var item in methods)
            list.Add(item.Name, MethodSignature.Create(item));
        return list;
    }
    #region Create
    /// <summary>
    /// 仅按方法名
    /// </summary>
    /// <param name="methods"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodContainer Create(params IEnumerable<IMethodSymbol> methods)
        => new(ToGroupList(methods));
    /// <summary>
    /// 按类型符号构造命名器
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodContainer Create(INamedTypeSymbol symbol)
        => Create(SymbolReflection.GetMethodsWithBase(symbol).Where(static item => !item.IsImplicitlyDeclared));
    /// <summary>
    /// 仅当前定义成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static MethodContainer DeclaredOnly(INamedTypeSymbol symbol)
         => Create(symbol.GetMembers().Where(static item => !item.IsImplicitlyDeclared).OfType<IMethodSymbol>());
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
        var @base = Create(SymbolReflection.GeNonPrivateMembersWithBase(baseSymbol).OfType<IMethodSymbol>());
        return new InheritProvider(original, @base);
    }
    #endregion
}
