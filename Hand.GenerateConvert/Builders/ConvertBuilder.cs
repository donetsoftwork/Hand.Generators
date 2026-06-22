using Hand.Cache;
using Hand.Converters;
using Hand.Enums;
using Hand.Members;
using Hand.Sources;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 转化构造器
/// </summary>
public partial class ConvertBuilder(Compilation compilation, List<IGeneratorSource> sources)
    : CacheFactoryBase<PairTypeSymbolKey, IConverter?>()
{
    /// <summary>
    /// 转化构造器
    /// </summary>
    /// <param name="compilation"></param>
    public ConvertBuilder(Compilation compilation)
        : this(compilation, [])
    {
    }
    #region 配置
    private readonly Compilation _compilation = compilation;
    private readonly List<IGeneratorSource> _sources = sources;
    private readonly SystemConvertProvider _systemConvertProvider = SystemConvertProvider.Create(compilation); 
    private readonly DefaultValueBuilder _valueBuilder = new(compilation);
    private readonly EnumBundleBuilder _bundleBuilder = new(compilation);
    //private readonly EnumBuilder _enumBuilder = new(this, _bundleBuilder);
    //private readonly ExtensionService extensionService = new(compilation, _compilation);
    /// <summary>
    /// 编译信息
    /// </summary>
    public Compilation Compilation 
        => _compilation;
    /// <summary>
    /// 转化源
    /// </summary>
    public IEnumerable<IGeneratorSource> Sources 
        => _sources;
    #endregion
    /// <summary>
    /// 添加转化源
    /// </summary>
    /// <param name="source"></param>
    public void AddSource(IGeneratorSource source)
        => _sources.Add(source);
    /// <summary>
    /// 获取转化器
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? Get(INamedTypeSymbol source, INamedTypeSymbol dest)
        => Get(new PairTypeSymbolKey(source, dest));
    /// <inheritdoc />
    protected override IConverter? CreateNew(in PairTypeSymbolKey key)
    {
        var (source, dest) = key;     
        var sourceInfo = MemberSymbolInfo.Create(_compilation, source);
        var destInfo = MemberSymbolInfo.Create(_compilation, dest);
        var converter = CreateCore(sourceInfo, destInfo);
        //var isNullable = NullCoalesceConverter.CheckNullable(sourceInfo.Category.IsNullable(), destInfo.Category.IsNullable());
        //if(isNullable)
        //    return new NullCoalesceConverter(converter, _valueBuilder.Get(destInfo));
        return converter;
    }
    /// <summary>
    /// 根据成员信息创建转化器
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private IConverter? CreateCore(MemberSymbolInfo sourceInfo, MemberSymbolInfo destInfo)
    {
        var (sourceSymbol, sourceCategory, sourceElement) = sourceInfo;
        var (destSymbol, destCategory, destElement) = destInfo;
        var sourceIsNull = sourceCategory.IsNullable();
        //var defaultValue = _valueBuilder.Get(destInfo);
        if (SymbolTypeDescriptor.CheckEquals(sourceSymbol, destSymbol))
            return new PassConverter(_valueBuilder.Get(destInfo), sourceIsNull);
        var conversion = _compilation.ClassifyCommonConversion(sourceSymbol, destSymbol);
        if (conversion.Exists)
        {
            if (conversion.IsImplicit)
                return new PassConverter(_valueBuilder.Get(destInfo), sourceIsNull);
            if (sourceCategory.IsEnum() && destCategory.IsEnum())
            {
                var original = EnumToEnum(sourceSymbol, destSymbol);
                if (original is not null)
                    return CheckSource(original, sourceIsNull, destInfo);
            }
            return CheckSource(new CastConverter(destSymbol.ToSyntax()), sourceIsNull, destInfo);
        }
        var stringSymbol = _compilation.GetStringSymbol();
        if (SymbolTypeDescriptor.CheckEquals(destSymbol, stringSymbol))
            return CheckSource(ToStringConverter.Instance, sourceIsNull, destInfo);
        var systemConvert = _systemConvertProvider.Get(sourceSymbol, destSymbol);
        if (systemConvert is not null)
            return CheckSource(systemConvert, sourceIsNull, destInfo);
        if (sourceCategory.IsEnum())
            return CheckOriginal(FromEnum(sourceSymbol, destSymbol), sourceIsNull, destInfo);
        if (destCategory.IsEnum())
            return CheckOriginal(ToEnum(sourceSymbol, destSymbol), sourceIsNull, destInfo);
        if (destCategory.IsEntity())
            return CheckOriginal(ToEntity(sourceSymbol, destSymbol), sourceIsNull, destInfo);

        throw new NotImplementedException();
    }
    /// <summary>
    /// 转化为实体
    /// </summary>
    /// <param name="sourceSymbol"></param>
    /// <param name="entitySymbol"></param>
    /// <returns></returns>
    public IConverter? ToEntity(INamedTypeSymbol sourceSymbol, INamedTypeSymbol entitySymbol)
    {
        var constructors = SymbolReflection.GetConstructors(sourceSymbol);
        var constructor = constructors.Where(m => SymbolTypeDescriptor.MatchFirst(m.Parameters, sourceSymbol))
            .OrderBy(m => m.Parameters.Length)
            .FirstOrDefault();
        if (constructor is not null)
        {
            if (constructor.Parameters.Length == 1)
                return new ConstructorConverter(entitySymbol.ToSyntax());
        }
        return null;
    }
    /// <summary>
    /// 判断原类型是否为空
    /// </summary>
    /// <param name="original"></param>
    /// <param name="isNull"></param>
    /// <param name="info"></param>
    /// <returns></returns>

    public IConverter CheckSource(IConverter original, bool isNull, MemberSymbolInfo info)
        => isNull ? new NullableConverter(original, _valueBuilder.Get(info)) : original;
    /// <summary>
    /// 判断原转化器是否存在
    /// </summary>
    /// <param name="original"></param>
    /// <param name="isNull"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public IConverter? CheckOriginal(IConverter? original, bool isNull, MemberSymbolInfo info)
        => original is null ? null : CheckSource(original, isNull, info);
    /// <summary>
    /// 获取静态方法
    /// </summary>
    /// <param name="declare"></param>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetStaticMethod(INamedTypeSymbol declare, INamedTypeSymbol source, INamedTypeSymbol dest, Func<IMethodSymbol, bool> filter)
        => GetMethod(SymbolReflection.GetMethods(declare).Where(m => SymbolTypeDescriptor.CheckEquals(dest, m.ReturnType) && SymbolTypeDescriptor.MatchFirst(m.Parameters, source)), filter);
    /// <summary>
    /// 获取实例方法
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetInstanceMethod(INamedTypeSymbol source, INamedTypeSymbol dest, Func<IMethodSymbol, bool> filter)
        => GetMethod(SymbolReflection.GetMethods(source).Where(m => SymbolTypeDescriptor.CheckEquals(dest, m.ReturnType)), filter);
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
