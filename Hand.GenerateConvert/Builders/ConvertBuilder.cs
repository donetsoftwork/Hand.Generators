using Hand.Attributes;
using Hand.Cache;
using Hand.Converters;
using Hand.Enums.Builders;
using Hand.Maping;
using Hand.Members;
using Hand.Providers;
using Hand.Sources;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 转化构造器
/// </summary>
public partial class ConvertBuilder(Compilation compilation, TypeInfoBuilder typeBuilder, SystemConvertProvider systemProvider, EnumBundleBuilder bundles, List<IGeneratorSource> sources)
    : CacheFactoryBase<PairSymbolInfoKey, ISyntaxConverter?>()
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
    private readonly TypeInfoBuilder _typeBuilder = typeBuilder;
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
    public TypeInfoBuilder TypeBuilder
        => _typeBuilder;
    /// <summary>
    /// 转化源
    /// </summary>
    public IReadOnlyCollection<IGeneratorSource> Sources 
        => _sources;
    #endregion
    /// <summary>
    /// 保存转化源
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="info"></param>
    public ISyntaxConverter Save(ComplexTypeInfo source, ComplexTypeInfo dest, ConvertSourceInfo info)
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
    /// 清空转化源
    /// </summary>
    public void ClearSource()
        => _sources.Clear();
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
    /// 添加复杂转化器
    /// </summary>
    /// <param name="method"></param>
    /// <param name="info"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public IGeneratorSource AddSource(ComplexSource method, TypeNameInfo info, INamedTypeSymbol symbol)
    {
        IGeneratorSource source;
        if (info.IsStatic)
        {
            source = new ExtensionMethodSource(method, info);
        }
        else
        {
            var type = SyntaxGenerator.TypeDeclaration(info.TypeName, symbol.IsRecord, symbol.IsValueType)
                .Partial();
            var containingNamespace = symbol.ContainingNamespace.ToDisplayString();
            SyntaxGenerator generator;
            if (string.IsNullOrWhiteSpace(containingNamespace))
                generator = SyntaxGenerator.Create(type);
            else
                generator = SyntaxGenerator.Create(containingNamespace, type);
            source = new ConvertToSource(this, generator, info.FullName, [method]);
        }
        _sources.Add(source);
        return source;
    }
    /// <summary>
    /// 获取转化器
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? Get(ITypeSymbol source, ITypeSymbol dest)
    {
        var sourceInfo = _typeBuilder.Get(source);
        if (sourceInfo is null)
            return null;
        var destInfo = _typeBuilder.Get(dest);
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
    public ISyntaxConverter? Get(ITypeSymbolInfo source, ITypeSymbolInfo dest)
        => Get(new PairSymbolInfoKey(source, dest));
    /// <inheritdoc />
    protected override ISyntaxConverter? CreateNew(in PairSymbolInfoKey key)
        => CreateCore(key.Left, key.Right);
    /// <summary>
    /// 根据成员信息创建转化器
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private ISyntaxConverter? CreateCore(ITypeSymbolInfo source, ITypeSymbolInfo dest)
    {
        // 原始类型兼容直接转化
        if (source.Original.IsCompatible(dest.Original))
            return new PassConverter(DefaultExpressionBuilder.GetDefault(dest), false);

        // 类型兼容直接转化
        if (source.Symbol.IsCompatible(dest.Symbol))
            return new PassConverter(DefaultExpressionBuilder.GetDefault(dest), source.IsNullable);

        return dest.Kind switch
        {
            TypeSymbolKind.Primitive => ToPrimitive(source, (PrimitiveTypeInfo)dest),
            TypeSymbolKind.Enum => ToEnum(source, (EnumTypeInfo)dest),
            TypeSymbolKind.Entity => ToEntity(source, (EntityPropertyTypeInfo)dest),
            TypeSymbolKind.Array => ToArray(source, (ArrayTypeInfo)dest),
            TypeSymbolKind.Collection => ToCollection(source, (CollectionTypeInfo)dest),
            TypeSymbolKind.Complex => ToComplex(source, (ComplexTypeInfo)dest),
            TypeSymbolKind.Enumeration => ToEnumeration(source, (EnumerationTypeInfo)dest),
            TypeSymbolKind.Generic => ToComplex(source, (ComplexTypeInfo)dest),
            TypeSymbolKind.Void => null,
            _ => ToUnknow(source, dest),
        };
    }
    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public static (ConvertSourceInfo, ISyntaxConverter?) GetConverter(Compilation compilation, INamedTypeSymbol source, INamedTypeSymbol dest)
    {
        var sourceProvider = SourceProvider.Create(compilation, source);
        var convertToInfo = sourceProvider.ConvertTo(dest.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, dest);
        if (convertToMethod is null)
            return (convertToInfo, null);
        return (convertToInfo, ConvertSourceInfo.GetConverter(convertToMethod));
    }
    /// <summary>
    /// 获取系统普通转化器(支持运算符重载)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? GetCommonConversion(ITypeSymbolInfo source, ITypeSymbolInfo dest)
    {
        var destSymbol = dest.Symbol;
        var conversion = _compilation.ClassifyCommonConversion(source.Symbol, destSymbol);
        if (!conversion.Exists)
            return null;
        if (conversion.IsImplicit)
            return new PassConverter(DefaultExpressionBuilder.GetDefault(dest), source.IsNullable);
        return CheckSource(new CastConverter(dest), source.IsNullable, dest);
    }
    /// <summary>
    /// 系统转化
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <returns></returns>
    public ISyntaxConverter? GetConverterBySystem(PrimitiveTypeInfo sourceInfo, PrimitiveTypeInfo destInfo)
    {
        var systemConvert = _systemProvider.Get(sourceInfo.Original, destInfo.Original);
        if (systemConvert is not null)
            return CheckSource(systemConvert, false, destInfo);
        var sourceIsNull = sourceInfo.IsNullable;
        var destIsNull = destInfo.IsNullable;
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
    public static ISyntaxConverter CheckSource(ISyntaxConverter original, bool isNull, ITypeSymbolInfo info)
        => isNull ? new NullableConverter(original, DefaultExpressionBuilder.GetDefault(info)) : original;
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
    public static string GetMethodSummary(ITypeSymbolInfo returnInfo)
    {
        var typeSummary = returnInfo.Summary;
        if (string.IsNullOrEmpty(typeSummary))
            return "转化";
        return "转化为" + typeSummary;
    }
    /// <summary>
    /// 获取成员字典
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="type"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IDictionary<string, IMemberInfo> GetSourceMembers(TypeInfoBuilder typeSymbols, INamedTypeSymbol type, IRecognizer<string>[] rules)
    {
        IDictionary<string, IMemberInfo> sourceMembers = SymbolMember.GetSourceMembers(typeSymbols, type);
        return Recognize(sourceMembers, rules);
    }
    /// <summary>
    /// 识别
    /// </summary>
    /// <param name="sourceMembers"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IDictionary<string, IMemberInfo> Recognize(IDictionary<string, IMemberInfo> sourceMembers, IRecognizer<string>[] rules)
    {
        foreach (var rule in rules)
            sourceMembers = rule.Recognize(sourceMembers);
        return sourceMembers;
    }
}
