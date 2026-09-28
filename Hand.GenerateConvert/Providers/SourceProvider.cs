using Hand.Members;
using Hand.Methods;
using Hand.Sources;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand.Providers;

/// <summary>
/// 生成源提供者
/// </summary>
/// <param name="typeInfo"></param>
/// <param name="symbolName"></param>
/// <param name="original"></param>
/// <param name="isPartial"></param>
/// <param name="isExtension"></param>
public class SourceProvider(TypeNameInfo typeInfo, string symbolName, IMethodProvider original, bool isPartial, bool isExtension = false)
    : IMethodProvider
{
    #region 配置
    private readonly Dictionary<PairTypeSymbolKey, IGeneratorSource> _sources = [];
    /// <summary>
    /// 类型信息
    /// </summary>
    private readonly TypeNameInfo _typeInfo = typeInfo;
    /// <summary>
    /// 当前类名
    /// </summary>
    private readonly string _symbolName = symbolName;
    /// <summary>
    /// 原始提供者
    /// </summary>
    private readonly IMethodProvider _original = original;
    /// <summary>
    /// 是否部分类
    /// </summary>
    private readonly bool _isPartial = isPartial;
    /// <summary>
    /// 是否扩展类
    /// </summary>
    private readonly bool _isExtension = isExtension;
    /// <summary>
    /// 类型信息
    /// </summary>
    public TypeNameInfo TypeInfo
     => _typeInfo;
    /// <summary>
    /// 当前类名
    /// </summary>
    public string SymbolName 
        => _symbolName;
    /// <summary>
    /// 是否部分类
    /// </summary>
    public bool IsPartial 
        => _isPartial;
    /// <summary>
    /// 是否扩展类
    /// </summary>
    public bool IsExtension 
        => _isExtension;
    #endregion
    #region ISourceProvider
    /// <inheritdoc />
    public ConvertSourceInfo ConvertTo(string dest)
    {
        if (_isExtension)
            return new(this, _typeInfo, ConvertMethodInfo.Create(dest, _symbolName, true, "To"), true, true, true);
        if (_isPartial)
            return new(this, _typeInfo, ConvertMethodInfo.Create(dest, _symbolName, false, "To"), true, false, false);
        return new(this, _typeInfo, ConvertMethodInfo.Create(dest, _symbolName, false, "To"), false, false, false);
    }
    ///// <inheritdoc />
    //public ConvertSourceInfo ConvertFrom(string source)
    //    => new(_typeInfo, ConvertMethodInfo.Create(source, _symbolName, "From"), _isPartial, true);
    /// <inheritdoc />
    public IGeneratorSource? Get(PairTypeSymbolKey key)
    {
        _sources.TryGetValue(key, out IGeneratorSource? generatorSource);
        return generatorSource;
    }
    /// <inheritdoc />
    public void Save(PairTypeSymbolKey key, IGeneratorSource source)
    {
        _sources[key] = source;
    }
    #endregion
    #region IMethodProvider
    ///// <summary>
    ///// 获取构造方法
    ///// </summary>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public IMethodSymbol? GetConvertMethod(INamedTypeSymbol dest)
    //{
    //    var convertToInfo = ConvertTo(dest.Name);
    //    return GetConvertMethod(convertToInfo.MethodInfo, dest);
    //}
    /// <summary>
    /// 获取构造方法
    /// </summary>
    /// <param name="info"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IMethodSymbol? GetConvertMethod(ConvertMethodInfo info, ITypeSymbol dest)
        => _original.GetConvertMethod(info, dest);
    #endregion
    #region Create
    /// <summary>
    /// 构造生成源提供者
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SourceProvider Create(Compilation compilation, INamedTypeSymbol symbol)
        => Create(compilation, symbol, SymbolEqualityComparer.Default.Equals(compilation.Assembly, symbol.ContainingAssembly) && symbol.IsPartial());
    /// <summary>
    /// 按扩展类构造生成源提供者
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public static SourceProvider CreateByExtension(Compilation compilation, INamedTypeSymbol symbol, bool isInternal)
    {
        var typeInfo = TypeNameInfo.GetExtensionInfo(symbol, isInternal);
        var extension = compilation.GetTypeByMetadataName(typeInfo.FullName);
        var original = MethodProvider.Create(symbol, extension);
        return new(typeInfo, symbol.Name, original, compilation.CheckIsPartial(extension), true);
    }
    /// <summary>
    /// 构造生成源提供者
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <param name="isPartial"></param>
    /// <returns></returns>
    public static SourceProvider Create(Compilation compilation, INamedTypeSymbol symbol, bool isPartial)
    {
        var isInternal = !SymbolEqualityComparer.Default.Equals(compilation.Assembly, symbol.ContainingAssembly)
            || symbol.DeclaredAccessibility != Accessibility.Public;
        if (isPartial)
        {
            // partial无需扩展类
            var typeInfo = TypeNameInfo.GetInfo(symbol, isInternal);
            var original = MethodProvider.Create(symbol, null);
            return new(typeInfo, symbol.Name, original, true, false);
        }
        return CreateByExtension(compilation, symbol, isInternal);
    }
    ///// <summary>
    ///// 检查扩展类是否是partial
    ///// </summary>
    ///// <param name="current"></param>
    ///// <param name="symbol"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static bool CheckTypeIsPartial(IAssemblySymbol current, INamedTypeSymbol? symbol)
    //    => symbol is null || !SymbolEqualityComparer.Default.Equals(current, symbol.ContainingAssembly) || symbol.IsPartial();
    #endregion
}
