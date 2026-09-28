using Hand.Attributes;
using Hand.Cache;
using Hand.Documentation;
using Hand.Enums.Bundles;
using Hand.Enums.Fields;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enums.Builders;

/// <summary>
/// 枚举信息构建器
/// </summary>
/// <param name="cacher"></param>
/// <param name="compilation"></param>
public class EnumBundleBuilder(ICacher<EnumTypeInfo, IEnumBundle> cacher, Compilation compilation)
    : CacheFactoryBase<EnumTypeInfo, IEnumBundle>(cacher)
{
    /// <summary>
    /// 枚举信息构建器
    /// </summary>
    /// <param name="compilation"></param>
    public EnumBundleBuilder(Compilation compilation)
        : this(new DictionaryCacher<EnumTypeInfo, IEnumBundle>(new Dictionary<EnumTypeInfo, IEnumBundle>(TypeInfoComparer.Default)), compilation)
    {
    }
    #region 配置
    /// <summary>
    /// 编译对象
    /// </summary>
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol? _enumMemberAttributeType = GetEnumMemberAttributeType(compilation);
    /// <summary>
    /// 编译对象
    /// </summary>
    public Compilation Compilation 
        => _compilation;
    #endregion
    #region CacheFactoryBase<MemberInfo, IMemberReader>
    /// <inheritdoc />
    protected override IEnumBundle CreateNew(in EnumTypeInfo key)
    {
        var symbol = key.Symbol;
        var fields = SymbolReflection.GetFields(symbol)
            .Where(field => field.IsStatic && field.HasConstantValue)
            .ToArray();
        var underType = key.Element.SpecialType;
        if (key.IsFlag)
        {
            var list = new List<FlagEnumField>(fields.Length);
            foreach (var field in fields)
                list.Add(CreateFlagField(underType, field));
            return new FlagEnumBundle(list);
        }
        else
        {
            var list = new List<EnumField>(fields.Length);
            foreach (var field in fields)
                list.Add(CreateField(underType, field));
            return new EnumBundle(list); ;
        }
    }
    #endregion
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
        var flag = System.Convert.ToUInt64(under);
        return new FlagEnumField(name, member, SyntaxGenerator.Literal(underType, under), flag, CommentParser.GetSummary(field));
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
        return new EnumField(name, member, SyntaxGenerator.Literal(underType, under), CommentParser.GetSummary(field));
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
    /// 获取FlagsAttribute特性类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagsAttributeType(Compilation compilation)
        => compilation.GetTypeByMetadataName("System.FlagsAttribute");
    /// <summary>
    /// 获取EnumMember特性类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetEnumMemberAttributeType(Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Runtime.Serialization.EnumMemberAttribute");
    #endregion
}