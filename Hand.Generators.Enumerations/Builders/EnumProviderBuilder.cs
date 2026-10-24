using Hand.Cache;
using Hand.Enumerations.Sources;
using Hand.Enums.Builders;
using Hand.Naming;
using Hand.Sources;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Enumerations.Builders;

/// <summary>
/// 反射枚举类型获取枚举提供器
/// </summary>
/// <param name="bundleBuilder"></param>
/// <param name="compilation"></param>
/// <param name="sources"></param>
public class EnumProviderBuilder(EnumBundleBuilder bundleBuilder, Compilation compilation, ISourceCollection sources)
    : CacheFactoryBase<EnumTypeInfo, EnumerationProvider?>(new DictionaryCacher<EnumTypeInfo, EnumerationProvider?>(new Dictionary<EnumTypeInfo, EnumerationProvider?>(TypeInfoComparer.Default)))
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="bundleBuilder"></param>
    /// <param name="sources"></param>
    public EnumProviderBuilder(EnumBundleBuilder bundleBuilder, ISourceCollection sources)
        : this(bundleBuilder, bundleBuilder.Compilation, sources)
    {
    }
    #region 配置
    private readonly EnumBundleBuilder _bundleBuilder = bundleBuilder;
    private readonly Compilation _compilation = compilation;
    private readonly ISourceCollection _sources = sources;

    /// <summary>
    /// 枚举基类
    /// </summary>
    private readonly INamedTypeSymbol? _enumerationSymbol = EnumerationTypeInfo.GetEnumerationBase(compilation);
    /// <summary>
    /// 标记位枚举基类
    /// </summary>
    private readonly INamedTypeSymbol? _flagEnumerationSymbol = EnumerationTypeInfo.GetFlagEnumerationBase(compilation);
    /// <summary>
    /// 枚举提供器接口
    /// </summary>
    private readonly INamedTypeSymbol? _genericInterface = EnumerationProviderBuilder.GetGenericInterface(compilation);
    /// <summary>
    /// 标记位枚举提供器接口
    /// </summary>
    private readonly INamedTypeSymbol? _genericFlagInterface = EnumerationProviderBuilder.GetFlagGenericInterface(compilation);
    /// <summary>
    /// 枚举提供器基类
    /// </summary>
    private readonly INamedTypeSymbol? _genericBase = EnumerationProviderBuilder.GetGenericBase(compilation);
    /// <summary>
    /// 标记位枚举提供器基类
    /// </summary>
    private readonly INamedTypeSymbol? _genericFlagBase = EnumerationProviderBuilder.GetFlagGenericBase(compilation);

    /// <summary>
    /// 枚举信息构建器
    /// </summary>
    public EnumBundleBuilder BundleBuilder 
        => _bundleBuilder;
    #endregion

    /// <inheritdoc />
    protected override EnumerationProvider? CreateNew(in EnumTypeInfo key)
    {
        var member = GetDisplay(key);
        if (member is null)
            return null;
        if (key.IsFlag)
            return EnumerationProvider.CreateFlag(member);
        return EnumerationProvider.CreateDefault(member);
    }
    /// <summary>
    /// 获取枚举类提供器
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ISyntaxDisplay<NameSyntax>? GetDisplay(EnumTypeInfo key)
    {
        var providerInterface = GetProviderInterface(key);
        if (providerInterface is null)
            return null;
        var enumSymbol = key.Symbol;
        var providerName = enumSymbol.Name + "Provider";
        var @namespace = enumSymbol.ContainingNamespace.ToDisplayString();
        var metadataName = TypeNameInfo.CheckFullName(@namespace, providerName);
        var providerSymbol = _compilation.GetTypeByMetadataName(metadataName);
        if (providerSymbol is null)
        {
            var source = key.IsFlag ? CreateFlagSource(enumSymbol) : CreateSource(enumSymbol);
            if (source is null)
                return null;
            _sources.Add(source);
            return source.GetInstanceMember();
        }
        return EnumerationProviderBuilder.GetProviderMember(providerSymbol, providerInterface);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumSymbol"></param>
    /// <returns></returns>
    public EnumProviderSource? CreateSource(INamedTypeSymbol enumSymbol)
    {
        if (_flagEnumerationSymbol is null || _genericBase is null)
            return null;
        var providerBase = _genericBase.Construct(_flagEnumerationSymbol);
        var providerBaseNaming = KindContainer.Create(providerBase);
        return EnumProviderSource.Create(enumSymbol, _bundleBuilder, providerBaseNaming);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumSymbol"></param>
    /// <returns></returns>
    public EnumProviderSource? CreateFlagSource(INamedTypeSymbol enumSymbol)
    {
        if (_enumerationSymbol is null || _genericFlagBase is null)
            return null;
        var providerBase = _genericFlagBase.Construct(_enumerationSymbol);
        var providerBaseNaming = KindContainer.Create(providerBase);
        return EnumProviderSource.CreateFlag(enumSymbol, _bundleBuilder, providerBaseNaming);
    }
    /// <summary>
    /// 获取提供器接口符号
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public INamedTypeSymbol? GetProviderInterface(EnumTypeInfo key)
    {
        if (key.IsFlag)
            return EnumerationProviderBuilder.GetProviderSymbol(key.Symbol, _flagEnumerationSymbol, _genericFlagInterface);
        return EnumerationProviderBuilder.GetProviderSymbol(key.Symbol, _enumerationSymbol, _genericInterface) ?? EnumerationProviderBuilder.GetProviderSymbol(key.Symbol, _flagEnumerationSymbol, _genericFlagInterface);
    }
}
