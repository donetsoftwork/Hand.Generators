using Hand.Cache;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Cachers;

/// <summary>
/// 类型信息缓存
/// </summary>
/// <param name="compilation"></param>
public class TypeSymbolCacher(Compilation compilation)
    : CacheFactoryBase<INamedTypeSymbol, TypeSymbolInfo?>(new DictionaryCacher<INamedTypeSymbol, TypeSymbolInfo?>(new Dictionary<INamedTypeSymbol, TypeSymbolInfo?>(SymbolEqualityComparer.Default)))
{
    #region 配置
    private readonly Compilation _compilation = compilation;
    /// <summary>
    /// 获取编译器
    /// </summary>
    public Compilation Compilation
        => _compilation;
    #endregion
    /// <inheritdoc />
    protected override TypeSymbolInfo? CreateNew(in INamedTypeSymbol key)
        => TypeSymbolInfo.Create(_compilation, key);
    /// <summary>
    /// 获取类型信息
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    public TypeSymbolInfo? GetByType(ITypeSymbol typeSymbol)
    {
        if(typeSymbol is INamedTypeSymbol namedTypeSymbol)
            return Get(namedTypeSymbol);
        return null;
    }
}
