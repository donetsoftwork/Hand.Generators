using Hand.Cache;
using Hand.Cachers;
using Hand.Converters;
using Hand.Enums;
using Hand.Maping;
using Hand.Members;
using Hand.Providers;
using Hand.Reflection;
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
public partial class ConvertBuilder(Compilation compilation, TypeSymbolCacher typeCacher, SystemConvertProvider systemProvider, EnumBundleBuilder bundles, List<IGeneratorSource> sources)
    : CacheFactoryBase<PairSymbolInfoKey, IConverter?>()
{
    /// <summary>
    /// 转化构造器
    /// </summary>
    /// <param name="compilation"></param>
    public ConvertBuilder(Compilation compilation)
        : this(compilation, new(compilation), SystemConvertProvider.Create(compilation), new(compilation), [])
    {
    }
    #region 配置
    private readonly Compilation _compilation = compilation;
    private readonly TypeSymbolCacher _typeCacher = typeCacher;
    private readonly SystemConvertProvider _systemProvider = systemProvider;
    private readonly EnumBundleBuilder _bundles = bundles;
    private readonly List<IGeneratorSource> _sources = sources;

    /// <summary>
    /// 编译信息
    /// </summary>
    public Compilation Compilation 
        => _compilation;
    /// <summary>
    /// 类型符号缓存器
    /// </summary>
    public TypeSymbolCacher TypeCacher
        => _typeCacher;
    /// <summary>
    /// 转化源
    /// </summary>
    public IEnumerable<IGeneratorSource> Sources 
        => _sources;
    #endregion
    /// <summary>
    /// 保存转化源
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="info"></param>
    public IConverter Save(TypeSymbolInfo source, TypeSymbolInfo dest, ConvertSourceInfo info)
    {
        var converter = info.Create();
        Save(new PairSymbolInfoKey(source, dest), converter);
        return converter;
    }
    /// <summary>
    /// 添加转化源
    /// </summary>
    /// <param name="source"></param>
    public void AddSource(IGeneratorSource source)
        => _sources.Add(source);
    /// <summary>
    /// 添加扩展方法转化源
    /// </summary>
    /// <param name="method"></param>
    /// <param name="info"></param>
    public ExtensionMethodSource AddSource(MethodSource method, TypeNameInfo info)
    {
        var source = new ExtensionMethodSource(method, info);
        _sources.Add(source);
        return source;
    }
    /// <summary>
    /// 获取转化器
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? Get(INamedTypeSymbol source, INamedTypeSymbol dest)
    {
        var sourceInfo = _typeCacher.Get(source);
        if (sourceInfo is null)
            return null;
        var destInfo = _typeCacher.Get(dest);
        if (destInfo is null)
            return null;
        return Get(new PairSymbolInfoKey(sourceInfo, destInfo));
    }
    /// <summary>
    /// 获取转化器
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? Get(TypeSymbolInfo source, TypeSymbolInfo dest)
        => Get(new PairSymbolInfoKey(source, dest));
    /// <inheritdoc />
    protected override IConverter? CreateNew(in PairSymbolInfoKey key)
        => CreateCore(key.Left, key.Right);
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public bool CheckCompatible(INamedTypeSymbol source, INamedTypeSymbol dest)
    //    => source.Equals(dest, SymbolEqualityComparer.Default);
    /// <summary>
    /// 根据成员信息创建转化器
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private IConverter? CreateCore(TypeSymbolInfo sourceInfo, TypeSymbolInfo destInfo)
    {
        var (sourceOriginal, sourceSymbol, sourceCategory, _) = sourceInfo;
        var (destOriginal, destSymbol, destCategory, _) = destInfo;

        if (sourceOriginal.Equals(destOriginal, SymbolEqualityComparer.Default))
            return new PassConverter(DefaultExpressionBuilder.Default(destInfo, _compilation), false);
        var sourceIsNull = sourceCategory.IsNullable();
        if (sourceSymbol.Equals(destSymbol, SymbolEqualityComparer.Default))
            return new PassConverter(DefaultExpressionBuilder.Default(destInfo, _compilation), sourceIsNull);

        var sourceProvider = SourceProvider.Create(_compilation, sourceSymbol);
        var convertToInfo = sourceProvider.ConvertTo(destSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, destSymbol);
        if (convertToMethod is not null)
            return convertToInfo.Create();

        var conversion = _compilation.ClassifyCommonConversion(sourceSymbol, destSymbol);
        if (conversion.Exists)
        {
            if (conversion.IsImplicit)
                return new PassConverter(DefaultExpressionBuilder.Default(destInfo, _compilation), sourceIsNull);
            if (sourceCategory.IsEnum() && destCategory.IsEnum())
            {
                var original = EnumToEnum(sourceSymbol, destInfo, convertToInfo);
                if (original is not null)
                    return CheckSource(original, sourceIsNull, destInfo);
            }
            return CheckSource(new CastConverter(destSymbol.ToSyntax()), sourceIsNull, destInfo);
        }
        var stringSymbol = _compilation.GetStringSymbol();
        if (destSymbol.Equals(stringSymbol, SymbolEqualityComparer.Default))
            return CheckSource(ToStringConverter.Instance, sourceIsNull, destInfo);

        var systemConvert = GetConverterBySystem(sourceInfo, destInfo);
        if (systemConvert is not null)
            return systemConvert;

        if (sourceCategory.IsEnum())
            return CheckOriginal(FromEnum(sourceSymbol, destInfo, convertToInfo), sourceIsNull, destInfo);
        if (destCategory.IsEnum())
            return CheckOriginal(ToEnum(sourceSymbol, destInfo, convertToInfo), sourceIsNull, destInfo);

        if (destCategory.IsComplex())
        {
            if (sourceCategory.IsComplex())
                return CheckOriginal(ComplexToComplex(sourceSymbol, destInfo, convertToInfo), sourceIsNull, destInfo);
            return CheckOriginal(ToComplex(sourceSymbol, destSymbol, convertToInfo), sourceIsNull, destInfo);
        }
        if (sourceCategory.IsComplex())
            return CheckOriginal(FromComplex(sourceSymbol, destSymbol, destInfo), sourceIsNull, destInfo);

        return null;
    }
    /// <summary>
    /// 系统转化
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <returns></returns>
    public IConverter? GetConverterBySystem(TypeSymbolInfo sourceInfo, TypeSymbolInfo destInfo)
    {
        var systemConvert = _systemProvider.Get(sourceInfo.Original, destInfo.Original);
        if (systemConvert is not null)
            return CheckSource(systemConvert, false, destInfo);
        var sourceIsNull = sourceInfo.Kind.IsNullable();
        var destIsNull = destInfo.Kind.IsNullable();
        if (sourceIsNull || destIsNull)
        {
            systemConvert = _systemProvider.Get(sourceInfo.Symbol, destInfo.Symbol);
            if (systemConvert is not null)
                return CheckSource(systemConvert, sourceIsNull, destInfo);
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
    public IConverter CheckSource(IConverter original, bool isNull, TypeSymbolInfo info)
        => isNull ? new NullableConverter(original, DefaultExpressionBuilder.Default(info, _compilation)) : original;
    /// <summary>
    /// 判断原转化器是否存在
    /// </summary>
    /// <param name="original"></param>
    /// <param name="isNull"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public IConverter? CheckOriginal(IConverter? original, bool isNull, TypeSymbolInfo info)
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
    /// <summary>
    /// 解析开关状态
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool CheckState(AttributeData attribute, string name, bool defaultValue)
    {
        var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, name);
        if (argument is null)
            return defaultValue;
        return argument.Value.GetPrimitive(defaultValue);
    }
    /// <summary>
    /// 解析目标类型符号
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? CheckToSymbol(AttributeData attribute)
    {
        var attributeType = attribute.AttributeClass;
        if (attributeType == null)
            return null;
        if (attributeType.IsGenericType)
            return attributeType.TypeArguments[0] as INamedTypeSymbol;
        return null;
        //var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, 0);
        //if (argument is null)
        //    return null;
        //return argument.Value.GetTypeSymbol();
    }
    /// <summary>
    /// 解析投影规则
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static IRecognizer<string>[] CheckRecognizeRules(AttributeData attribute)
    {
        var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, "Rules");
        if (argument is null)
            return [];
        var values = argument.Value.Values;
        var count = values.Length;
        if (count == 0)
            return [];
        var list = new List<IRecognizer<string>>(count);
        foreach (var value in values)
        {
            var text = value.GetPrimitive<string>();
            if (string.IsNullOrEmpty(text))
                continue;
            list.Add(MemberRecognizeParser.Default.Parse(text));
        }
        return [.. list];
    }
    /// <summary>
    /// 获取方法备注
    /// </summary>
    /// <param name="returnInfo"></param>
    /// <returns></returns>
    public static string GetMethodSummary(TypeSymbolInfo returnInfo)
    {
        var typeSummary = returnInfo.Summary;
        if (string.IsNullOrEmpty(typeSummary))
            return "转化";
        return "转化为" + typeSummary;
    }
}
