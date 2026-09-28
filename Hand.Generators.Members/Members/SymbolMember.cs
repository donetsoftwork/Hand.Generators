using Hand.Builders;
using Hand.Documentation;
using Hand.Fields;
using Hand.Parameters;
using Hand.Properties;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Hand.Members;

/// <summary>
/// 符号成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
/// <param name="summary"></param>
public abstract class SymbolMember(string name, ISymbol original, ITypeSymbolInfo symbolInfo, Lazy<string> summary)
    : MemberBase(name, symbolInfo)
{
    /// <summary>
    /// 符号成员
    /// </summary>
    /// <param name="name"></param>
    /// <param name="original"></param>
    /// <param name="symbolInfo"></param>
    public SymbolMember(string name, ISymbol original, ITypeSymbolInfo symbolInfo)
        : this(name, original, symbolInfo, new Lazy<string>(() => SummaryCacher.GetSummary(original, symbolInfo.Summary)))
    {
    }

    #region 配置
    private readonly ISymbol _original = original;
    private readonly Lazy<string> _summary = summary;
    private readonly Lazy<XmlElementSyntax?> _xmlElement = new (() => SyntaxGenerator.CreateSummary(summary.Value));
    /// <summary>
    /// 原始成员
    /// </summary>
    public ISymbol Original 
        => _original;
    /// <inheritdoc />
    public override string Summary 
        => _summary.Value;
    /// <inheritdoc />
    public override XmlElementSyntax? XmlElement
        => _xmlElement.Value;
    /// <inheritdoc />
    public override ImmutableArray<AttributeData> GetAttributes()
        => _original.GetAttributes();
    #endregion
    /// <summary>
    /// 获取成员
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="type"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public static IDictionary<string, IMemberInfo> GetTargetMembers(TypeInfoBuilder typeSymbols, INamedTypeSymbol type, bool isPublic = true)
    {
        var constructor = SymbolReflection.GetConstructors(type)
            // 过滤第一个参数为自身的构造函数
            .Where(c => !SymbolReflection.MatchFirst(c.Parameters, type))
            .OrderBy(c => c.Parameters.Length)
            .FirstOrDefault();
        IDictionary<string, IMemberInfo> members = ParameterMember.GetDictionary(typeSymbols, constructor);
        CheckMembers(typeSymbols, members, type, WritePropertyFilter, isPublic);
        return members;
    }
    /// <summary>
    /// 构造来源成员字典
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="source"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public static Dictionary<string, IMemberInfo> GetSourceMembers(TypeInfoBuilder typeSymbols, INamedTypeSymbol source, bool isPublic = true)
    {
        var members = new Dictionary<string, IMemberInfo>(StringComparer.OrdinalIgnoreCase‌);
        CheckMembers(typeSymbols, members, source, ReadPropertyFilter, isPublic);
        return members;
    }
    /// <summary>
    /// 可读属性过滤条件
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool ReadPropertyFilter(IPropertySymbol property)
        => !property.IsIndexer && !property.IsStatic && !property.IsWriteOnly;
    /// <summary>
    /// 可读属性过滤条件
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static bool WritePropertyFilter(IPropertySymbol property)
        => !property.IsIndexer && !property.IsStatic && !property.IsReadOnly;
    /// <summary>
    /// 获取成员(字段和属性)
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="members"></param>
    /// <param name="type"></param>
    /// <param name="propertyFilter"></param>
    /// <param name="isPublic"></param>
    public static void CheckMembers(TypeInfoBuilder typeSymbols, IDictionary<string, IMemberInfo> members, INamedTypeSymbol type, Func<IPropertySymbol, bool> propertyFilter, bool isPublic = true)
    {
        IEnumerable<IFieldSymbol> fields;
        IEnumerable<IPropertySymbol> properties;
        if (isPublic)
        {
            fields = SymbolReflection.GetPublicFieldsWithBase(type);
            properties = SymbolReflection.GetPublicPropertiesWithBase(type);
        }
        else
        {
            fields = SymbolReflection.GetFieldsWithBase(type);
            properties = SymbolReflection.GetPropertiesWithBase(type);
        }
        FieldMember.CheckFields(typeSymbols, members, fields.Where(static field => !field.IsStatic));
        PropertyMember.CheckProperties(typeSymbols, members, properties.Where(propertyFilter));
    }
}
