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
/// 复合命名器
/// </summary>
/// <param name="methods"></param>
/// <param name="other"></param>
public sealed class CompositeProvider(MethodContainer methods, INamingProvider other)
    : INamingProvider
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CompositeProvider()
        : this(new MethodContainer(), new KindContainer())
    {
    }
    #region 配置
    /// <summary>
    /// 方法命名器
    /// </summary>
    private readonly MethodContainer _methods = methods;
    /// <summary>
    /// 其他成员命名器
    /// </summary>
    private readonly INamingProvider _other = other;
    #endregion
    #region INamingProvider
    /// <inheritdoc />
    public bool Contains(string name)
        => _other.Contains(name) || _methods.Contains(name);
    /// <inheritdoc />
    public void AddKind(string name, MemberKind kind)
        => _other.AddKind(name, kind);
    /// <inheritdoc />
    public bool TryDeclareMethod(string name, params ITypeSymbol[] parameterSymbols)
        => !_other.Contains(name) && _methods.TryDeclareMethod(name, parameterSymbols);
    /// <inheritdoc />
    public bool TryDeclareMethod(MethodSignature method)
        => !_other.Contains(method.Name) && _methods.TryDeclareMethod(method);
    /// <inheritdoc />
    public bool TryGetMemberKind(string name, out MemberKind kind)
         => _other.TryGetMemberKind(name, out kind) || _methods.TryGetMemberKind(name, out kind);
    /// <inheritdoc />
    public bool Contains(MethodSignature method)
        => _methods.Contains(method);
    #endregion
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    public void Add(MethodSignature method)
        => _methods.Add(method);
    /// <summary>
    /// 验证构造函数是否已经存在
    /// </summary>
    /// <returns></returns>
    public bool HasConstructor()
        => _methods.HasConstructor();
    /// <summary>
    /// 验证构造函数是否已经存在
    /// </summary>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    public bool ConstructorContains(params ITypeSymbol[] parameterSymbols)
        => _methods.ConstructorContains(parameterSymbols);
    ///// <summary>
    ///// 验证方法是否已经存在
    ///// </summary>
    ///// <param name="methodName"></param>
    ///// <param name="parameters"></param>
    ///// <returns></returns>
    //public bool MethodContains(string methodName, params IParameterInfo[] parameters)
    //    => _methods.MethodContains(methodName, parameters);
    /// <summary>
    /// 验证方法是否已经存在
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public bool MethodContains(string methodName, params ITypeSymbol[] parameterTypes)
        => _methods.MethodContains(methodName, parameterTypes);
    /// <summary>
    /// 转化为字典
    /// </summary>
    /// <param name="members"></param>
    /// <returns></returns>
    public static (Dictionary<string, MemberKind>, GroupList<MethodSignature>) ToDictionary(params IEnumerable<ISymbol> members)
    {
        var dictionary = new Dictionary<string, MemberKind>();
        var list = new GroupList<MethodSignature>();
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
                    if (item is IMethodSymbol method)
                        list.Add(method.Name, MethodSignature.Create(method));
                    break;
                default:
                    break;
            }
        }
        return (dictionary, list);
    }
    #region Create
    /// <summary>
    /// 按成员构造复合命名器
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static CompositeProvider Create(string typeName, params IEnumerable<ISymbol> members)
    {
        var(dictionary, list) = ToDictionary(members);
        var kinds = new KindContainer(dictionary);
        var methods = new MethodContainer(list);
        return new(methods, new TypedProvider(typeName, kinds));
    }
    /// <summary>
    /// 按类型符号构造命名器
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompositeProvider Create(INamedTypeSymbol symbol)
        => Create(symbol.Name, SymbolReflection.GetMembersWithBase(symbol).Where(static item => !item.IsImplicitlyDeclared));
    /// <summary>
    /// 仅当前定义成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static CompositeProvider DeclaredOnly(INamedTypeSymbol symbol)
         => Create(symbol.Name, symbol.GetMembers().Where(static item => !item.IsImplicitlyDeclared));
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
        var original = DeclaredOnly( symbol);
        var (dictionary, list) = ToDictionary(SymbolReflection.GeNonPrivateMembersWithBase(baseSymbol));
        var kinds = new KindContainer(dictionary);
        var methods = new MethodContainer(list);
        var @base = new CompositeProvider(methods, kinds);
        return new InheritProvider(original, @base);
    }
    #endregion
}
