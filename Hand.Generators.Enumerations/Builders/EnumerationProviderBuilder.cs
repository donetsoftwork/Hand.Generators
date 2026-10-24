using Hand.Cache;
using Hand.Enumerations.Sources;
using Hand.Members;
using Hand.Naming;
using Hand.Reflection;
using Hand.Sources;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enumerations.Builders;

/// <summary>
/// 获取枚举类提供器
/// </summary>
/// <param name="compilation"></param>
/// <param name="sources"></param>
public class EnumerationProviderBuilder(Compilation compilation, ISourceCollection sources)
    : CacheFactoryBase<EnumerationTypeInfo, EnumerationProvider?>(new DictionaryCacher<EnumerationTypeInfo, EnumerationProvider?>(new Dictionary<EnumerationTypeInfo, EnumerationProvider?>(TypeInfoComparer.Default)))
{

    private const string _genericInterfaceMetadataName = "Hand.Enumerations.IEnumerationProvider`1";
    private const string _genericFlagInterfaceMetadataName = "Hand.Enumerations.IFlagEnumerationProvider`1";

    private const string _genericBaseMetadataName = "Hand.Enumerations.EnumerationProvider`1";
    private const string _genericFlagBaseMetadataName = "Hand.Enumerations.FlagEnumerationProvider`1";

    private readonly Compilation _compilation = compilation;
    private readonly ISourceCollection _sources = sources;
    /// <summary>
    /// 枚举提供器接口
    /// </summary>
    private readonly INamedTypeSymbol? _genericInterface = compilation.GetTypeByMetadataName(_genericInterfaceMetadataName);
    /// <summary>
    /// 标记位枚举提供器
    /// </summary>
    private readonly INamedTypeSymbol? _genericFlagInterface = compilation.GetTypeByMetadataName(_genericFlagInterfaceMetadataName);

    /// <inheritdoc />
    protected override EnumerationProvider? CreateNew(in EnumerationTypeInfo key)
    {
        var member = GetDisplay(key);
        if (member is null)
            return null;
        if (key.IsFlag)
            return EnumerationProvider.CreateFlag(member);
        return EnumerationProvider.CreateDefault(member);
    }
    /// <summary>
    /// 获取枚举提供器
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ISyntaxDisplay<NameSyntax>? GetDisplay(EnumerationTypeInfo key)
    {
        var providerInterface = GetProviderInterface(key);
        if (providerInterface is null)
            return null;
        var enumerationSymbol = key.Symbol;
        var member = GetProviderMember(enumerationSymbol, providerInterface);
        if (member is null)
            return GetDisplayByProvider(enumerationSymbol, key.IsFlag, providerInterface);
        return member;
    }
    /// <summary>
    /// 获取枚举提供器
    /// </summary>
    /// <param name="enumerationSymbol"></param>
    /// <param name="isFlag"></param>
    /// <param name="providerInterface"></param>
    /// <returns></returns>
    public ISyntaxDisplay<NameSyntax>? GetDisplayByProvider(INamedTypeSymbol enumerationSymbol, bool isFlag, INamedTypeSymbol providerInterface)
    {
        var providerName = enumerationSymbol.Name + "Provider";
        var @namespace = enumerationSymbol.ContainingNamespace.ToDisplayString();
        var metadataName = TypeNameInfo.CheckFullName(@namespace, providerName);
        var providerSymbol = _compilation.GetTypeByMetadataName(metadataName);
        if (providerSymbol is null)
        {
            var baseSymbol = providerInterface.Construct(enumerationSymbol);
            var baseNaming = KindContainer.Create(baseSymbol);
            var naming = new TypedProvider(providerName).Inherit(baseNaming);
            var source = isFlag? EnumerationProviderSource.CreateFlag(enumerationSymbol, naming) : EnumerationProviderSource.Create(enumerationSymbol, naming);
            if (source is null)
                return null;
            _sources.Add(source);
            //return new StaticMember(source.Namespace, source.ProviderName, SyntaxFactory.IdentifierName(EnumerationProviderSource.InstanceName));
            return source.GetInstanceMember();
        }
        return GetProviderMember(providerSymbol, providerInterface);
    }
    /// <summary>
    /// 获取枚举提供器成员
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="providerInterface"></param>
    /// <returns></returns>
    public static ISyntaxDisplay<NameSyntax>? GetProviderMember(INamedTypeSymbol symbol, INamedTypeSymbol providerInterface)
    {
        var member = GetProviderField(symbol, providerInterface) ?? GetProviderProperty(symbol, providerInterface);
        if (member is null)
            return null;
        return StaticMember.Create(member);
    }
    /// <summary>
    /// 从静态字段中获取提供器
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="providerInterface"></param>
    /// <returns></returns>
    public static ISymbol? GetProviderField(INamedTypeSymbol symbol, INamedTypeSymbol providerInterface)
    {
        return SymbolReflection.GetFields(symbol)
            .Where(item => item.DeclaredAccessibility == Accessibility.Public && item.IsStatic && item.Type.IsInterface(providerInterface))
            .FirstOrDefault();
    }
    /// <summary>
    /// 从静态属性中获取提供器
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="providerInterface"></param>
    /// <returns></returns>
    public static ISymbol? GetProviderProperty(INamedTypeSymbol symbol, INamedTypeSymbol providerInterface)
    {
        return SymbolReflection.GetProperties(symbol)
            .Where(item => item.DeclaredAccessibility == Accessibility.Public && item.IsStatic && item.Type.IsInterface(providerInterface))
            .FirstOrDefault();
    }

    /// <summary>
    /// 获取提供器接口符号
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public INamedTypeSymbol? GetProviderInterface(EnumerationTypeInfo key)
    {
        if (key.IsInterface)
            return null;
        if (key.IsFlag)
            return GetProviderSymbol(key.Symbol, key.Symbol, _genericFlagInterface);
        return GetProviderSymbol(key.Symbol, key.Symbol, _genericInterface);
    }
    /// <summary>
    /// 获取提供器接口符号
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="enumeration"></param>
    /// <param name="genericProvider"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetProviderSymbol(INamedTypeSymbol symbol, INamedTypeSymbol? enumeration, INamedTypeSymbol? genericProvider)
    {
        if (genericProvider is null)
            return null;
        if (SymbolEqualityComparer.Default.Equals(symbol, enumeration))
            return null;
        return genericProvider.Construct(symbol);
    }
    ///// <summary>
    ///// 标识转化为字段
    ///// </summary>
    ///// <param name="generator"></param>
    ///// <param name="fieldType"></param>
    ///// <param name="fieldName"></param>
    ///// <param name="expression"></param>
    ///// <returns></returns>
    //public static void CreateField(SyntaxGenerator generator, TypeSyntax fieldType, SyntaxToken fieldName, ExpressionSyntax expression)
    //{
    //    var field = fieldType.Field(fieldName, expression)
    //        .Public()
    //        .Static()
    //        .ReadOnly();
    //    generator.AddField(field);
    //}
    #region GetSymbol
    /// <summary>
    /// 获取提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetGenericInterface(Compilation compilation)
        => compilation.GetTypeByMetadataName(_genericInterfaceMetadataName);
    /// <summary>
    /// 获取提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="enumeration"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetInterface(Compilation compilation, INamedTypeSymbol enumeration)
        => compilation.GetTypeByMetadataName(_genericInterfaceMetadataName)?.Construct(enumeration);
    /// <summary>
    /// 获取位标记提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagGenericInterface(Compilation compilation)
        => compilation.GetTypeByMetadataName(_genericFlagInterfaceMetadataName);
    /// <summary>
    /// 获取位标记提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="enumeration"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagInterface(Compilation compilation, INamedTypeSymbol enumeration)
        => compilation.GetTypeByMetadataName(_genericFlagInterfaceMetadataName)?.Construct(enumeration);
    /// <summary>
    /// 获取提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetGenericBase(Compilation compilation)
        => compilation.GetTypeByMetadataName(_genericBaseMetadataName);
    /// <summary>
    /// 获取提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="enumeration"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetSymbol(Compilation compilation, INamedTypeSymbol enumeration)
        => compilation.GetTypeByMetadataName(_genericBaseMetadataName)?.Construct(enumeration);
    /// <summary>
    /// 获取位标记提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagGenericBase(Compilation compilation)
        => compilation.GetTypeByMetadataName(_genericFlagBaseMetadataName);
    /// <summary>
    /// 获取位标记提供器接口
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="enumeration"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagSymbol(Compilation compilation, INamedTypeSymbol enumeration)
        => compilation.GetTypeByMetadataName(_genericFlagBaseMetadataName)?.Construct(enumeration);
    #endregion
}
