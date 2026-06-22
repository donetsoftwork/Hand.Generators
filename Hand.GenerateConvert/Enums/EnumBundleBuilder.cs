using Hand.Cache;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enums;

/// <summary>
/// 枚举信息构建器
/// </summary>
/// <param name="cacher"></param>
/// <param name="compilation"></param>
public class EnumBundleBuilder(ICacher<INamedTypeSymbol, IEnumBundle> cacher, Compilation compilation)
    : CacheFactoryBase<INamedTypeSymbol, IEnumBundle>(cacher)
{
    /// <summary>
    /// 枚举信息构建器
    /// </summary>
    /// <param name="compilation"></param>
    public EnumBundleBuilder(Compilation compilation)
        : this(new DictionaryCacher<INamedTypeSymbol, IEnumBundle>(new Dictionary<INamedTypeSymbol, IEnumBundle>(SymbolEqualityComparer.Default)), compilation)
    {
    }
    #region 配置
    /// <summary>
    /// 编译对象
    /// </summary>
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol? _flagsAttributeType = GetFlagsAttributeType(compilation);
    private readonly INamedTypeSymbol? _enumMemberAttributeType = GetEnumMemberAttributeType(compilation);
    /// <summary>
    /// 编译对象
    /// </summary>
    public Compilation Compilation 
        => _compilation;
    #endregion
    #region CacheFactoryBase<MemberInfo, IMemberReader>
    /// <inheritdoc />
    protected override IEnumBundle CreateNew(in INamedTypeSymbol key)
    {
        var underType = key.EnumUnderlyingType!;
        var fields = SymbolReflection.GetFields(key)
            .Where(field => field.IsStatic && field.HasConstantValue)
            .ToArray();
        if (IsFlagEnum(key))
        {
            FlagEnumBundle flagBundle = new(key, underType, fields.Length);
            CheckFields(flagBundle, fields);
            return flagBundle;
        }
        EnumBundle bundle = new(key, underType, fields.Length);
        CheckFields(bundle, fields);
        return bundle;
    }
    #endregion
    private void CheckFields(EnumBundle bundle, IFieldSymbol[] fields)
    {
        //var enumType = SyntaxFactory.IdentifierName(bundle.EnumType.Name);
        var underType = bundle.UnderType.SpecialType;
        foreach (var field in fields)
            bundle.AddField(CreateField(underType, field));
    }
    private void CheckFields(FlagEnumBundle bundle, IFieldSymbol[] fields)
    {
        //var enumType = SyntaxFactory.IdentifierName(bundle.EnumType.Name);
        var underType = bundle.UnderType.SpecialType;
        foreach (var field in fields)
            bundle.AddField(CreateFlagField(underType, field));
    }
    /// <summary>
    /// 构建枚举字段
    /// </summary>
    /// <param name="underType"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private FlagEnumField CreateFlagField(SpecialType underType, IFieldSymbol field)
    {
        var name = field.Name;
        var under = field.ConstantValue!;
        var member = GetEnumMemberName(field);
        var flag = Convert.ToUInt64(under);
        return new FlagEnumField(name, member/*, enumType.Access(name)*/, SyntaxGenerator.Literal(underType, under), flag);
    }
    /// <summary>
    /// 构建枚举字段
    /// </summary>
    /// <param name="underType"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private EnumField CreateField(SpecialType underType, IFieldSymbol field)
    {
        var name = field.Name;
        var under = field.ConstantValue!;
        var member = GetEnumMemberName(field);
        return new EnumField(name, member/*, enumType.Access(name)*/, SyntaxGenerator.Literal(underType, under));
    }
    #region Attribute
    /// <summary>
    /// 获取枚举成员名称
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public string GetEnumMemberName(IFieldSymbol field)
    {
        if (_enumMemberAttributeType is null)
            return string.Empty;
        var attribute = SymbolAttributeHelper.GetAttributesByType(field, _enumMemberAttributeType)
            .FirstOrDefault();
        if (attribute is null)
            return string.Empty;
        return SymbolAttributeHelper.GetArgumentValue<string>(attribute, "Value") ?? string.Empty;
    }
    /// <summary>
    /// 判断是否为标志枚举
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool IsFlagEnum(INamedTypeSymbol type)
    {
        if (_flagsAttributeType is null)
            return false;
        return SymbolAttributeHelper.GetAttributesByType(type, _flagsAttributeType)
            .Any();
    }
    /// <summary>
    /// 获取EnumMember特性类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagsAttributeType(Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Runtime.Serialization.FlagsAttribute");
    /// <summary>
    /// 获取EnumMember特性类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetEnumMemberAttributeType(Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Runtime.Serialization.EnumMemberAttribute");
    #endregion
}