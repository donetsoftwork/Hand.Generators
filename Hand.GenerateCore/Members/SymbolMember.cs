using Hand.Cachers;
using Hand.Reflection;
using Hand.Symbols;
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
public abstract class SymbolMember(string name, ISymbol original, TypeSymbolInfo symbolInfo, Lazy<string> summary)
    : Member(name, symbolInfo, summary, new Lazy<XmlElementSyntax?>(() => SyntaxGenerator.CreateSummary(summary.Value)))
{
    /// <summary>
    /// 符号成员
    /// </summary>
    /// <param name="name"></param>
    /// <param name="original"></param>
    /// <param name="symbolInfo"></param>
    public SymbolMember(string name, ISymbol original, TypeSymbolInfo symbolInfo)
        : this(name, original, symbolInfo, new Lazy<string>(() => SummaryCacher.GetSummary(original, symbolInfo.Summary)))
    {
    }

    #region 配置
    private readonly ISymbol _original = original;
    /// <summary>
    /// 原始成员
    /// </summary>
    public ISymbol Original 
        => _original;
    /// <summary>
    /// 获取特性标记
    /// </summary>
    /// <returns></returns>
    public ImmutableArray<AttributeData> GetAttributes()
        => _original.GetAttributes();
    #endregion
    /// <summary>
    /// 获取成员
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="type"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public static Dictionary<string, SymbolMember> GetTargetMembers(TypeSymbolCacher typeSymbols, INamedTypeSymbol type, bool isPublic = true)
    {
        var constructor = SymbolReflection.GetConstructors(type)
            // 过滤第一个参数为自身的构造函数
            .Where(c => !SymbolTypeDescriptor.MatchFirst(c.Parameters, type))
            .OrderBy(c => c.Parameters.Length)
            .FirstOrDefault();
        var members = ParameterMember.GetMethodParameters(typeSymbols, constructor);
        CheckMembers(typeSymbols, members, type, static property => !property.IsIndexer && !property.IsStatic && !property.IsReadOnly, isPublic);
        return members;
    }
    /// <summary>
    /// 构造来源成员字典
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="source"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    public static Dictionary<string, SymbolMember> GetSourceMembers(TypeSymbolCacher typeSymbols, INamedTypeSymbol source, bool isPublic = true)
    {
        var members = new Dictionary<string, SymbolMember>(StringComparer.OrdinalIgnoreCase‌);
        CheckMembers(typeSymbols, members, source, static property => !property.IsIndexer && !property.IsStatic && !property.IsWriteOnly, isPublic);
        return members;
    }
    /// <summary>
    /// 获取成员(字段和属性)
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="members"></param>
    /// <param name="type"></param>
    /// <param name="propertyFilter"></param>
    /// <param name="isPublic"></param>
    public static void CheckMembers(TypeSymbolCacher typeSymbols, Dictionary<string, SymbolMember> members, INamedTypeSymbol type, Func<IPropertySymbol, bool> propertyFilter, bool isPublic = true)
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
