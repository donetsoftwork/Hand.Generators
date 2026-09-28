using Hand.Methods;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Providers;

/// <summary>
/// 实例方法提供者
/// </summary>
/// <param name="symbol"></param>
public class MethodProvider(INamedTypeSymbol symbol)
    : IMethodProvider
{
    #region 配置
    private readonly INamedTypeSymbol _symbol = symbol;
    #endregion

    /// <inheritdoc />
    IMethodSymbol? IMethodProvider.GetConvertMethod(ConvertMethodInfo info, ITypeSymbol dest)
        => GetInstanceMethod(_symbol, dest, info.Filter);

    /// <summary>
    /// 构造方法提供者
    /// </summary>
    /// <param name="source"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static IMethodProvider Create(INamedTypeSymbol source, INamedTypeSymbol? extension = null)
    {
        var isPrimitiveType = source.IsPrimitiveType();
        if (extension is null)
        {
            if (isPrimitiveType)
                return MethodEmptyProvider.Instance;
            return new MethodProvider(source);
        }
        if (isPrimitiveType)
            return new StaticMethodProvider(extension, source);
        return new ComplexMethodProvider(new MethodProvider(source), new StaticMethodProvider(extension, source));
    }
    /// <summary>
    /// 获取静态方法
    /// </summary>
    /// <param name="declare"></param>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetStaticMethod(INamedTypeSymbol declare, INamedTypeSymbol source, ITypeSymbol dest, Func<IMethodSymbol, bool> filter)
        => GetMethod(SymbolReflection.GetMethods(declare).Where(m => dest.Equals(m.ReturnType, SymbolEqualityComparer.Default) && SymbolReflection.MatchFirst(m.Parameters, source)), filter);
    /// <summary>
    /// 获取实例方法
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetInstanceMethod(INamedTypeSymbol source, ITypeSymbol dest, Func<IMethodSymbol, bool> filter)
        => GetMethod(SymbolReflection.GetMethods(source).Where(m => dest.Equals(m.ReturnType, SymbolEqualityComparer.Default)), filter);
    /// <summary>
    /// 获取参数最好的方法
    /// </summary>
    /// <param name="methods"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetMethod(IEnumerable<IMethodSymbol> methods, Func<IMethodSymbol, bool> filter)
        => methods.OrderBy(m => m.Parameters.Length)
        .FirstOrDefault(filter);
}
