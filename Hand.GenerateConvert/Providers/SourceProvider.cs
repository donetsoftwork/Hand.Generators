using Hand.Members;
using Hand.Sources;
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
    /// 当前类名
    /// </summary>
    public string SymbolName 
        => _symbolName;
    #endregion
    #region ISourceProvider
    /// <inheritdoc />
    public ConvertSourceInfo ConvertTo(string dest)
    {
        //string destName = dest.Name;
        //var info = ConvertMethodInfo.Create(dest, _symbolName, _isExtension, "To");
        //GetConvertMethod(info, dest)
        return new(this, _typeInfo, ConvertMethodInfo.Create(dest, _symbolName, _isExtension, "To"), _isPartial, _isExtension);
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
    public IMethodSymbol? GetConvertMethod(ConvertMethodInfo info, INamedTypeSymbol dest)
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
    /// <returns></returns>
    public static SourceProvider CreateByExtension(Compilation compilation, INamedTypeSymbol symbol)
    {
        var typeInfo = TypeNameInfo.GetExtensionInfo(symbol);
        var extension = compilation.GetTypeByMetadataName(typeInfo.FullName);
        var original = MethodProvider.Create(symbol, extension);
        return new(typeInfo, symbol.Name, original, CheckIsPartial(compilation.Assembly, extension), true);
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
        if(isPartial)
        {
            // partial无需扩展类
            var typeInfo = TypeNameInfo.GetInfo(symbol);
            var original = MethodProvider.Create(symbol, null);
            return new(typeInfo, symbol.Name, original, true, false);
        }
        return CreateByExtension(compilation, symbol);
    }
    /// <summary>
    /// 检查扩展类是否是partial
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckIsPartial(IAssemblySymbol assembly, INamedTypeSymbol? extension)
        => extension is null || !SymbolEqualityComparer.Default.Equals(assembly, extension.ContainingAssembly) || extension.IsPartial();
    #endregion
}
