using Hand.Arguments;
using Hand.Converters;
using Hand.Converters.Constructors;
using Hand.Converters.Members;
using Hand.Members;
using Hand.Reflection;
using Hand.Sources;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 复杂类型转化
/// </summary>
public partial class ConvertBuilder
{
    #region ToComplex
    /// <summary>
    /// 转化为复杂类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToComplex(ITypeSymbolInfo source, ComplexTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Complex => ComplexToComplex((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Generic => ComplexToComplex((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Enum => EnumToComplex((EnumTypeInfo)source, dest),
            TypeSymbolKind.Enumeration => ComplexToComplex((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToComplex((EntityPropertyTypeInfo)source, dest),
            TypeSymbolKind.Array => CollectionToOther((ICollectionTypeInfo)source, dest),
            TypeSymbolKind.Collection => CollectionToOther((ICollectionTypeInfo)source, dest),
            TypeSymbolKind.Void => null,
            _ => OtherToComplex(source, dest),
        };
    }
    /// <summary>
    /// 复杂类型转化为复杂类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ComplexToComplex(ComplexTypeInfo source, ComplexTypeInfo dest)
    {
        (var convertToInfo, var converter) = GetConverter(_compilation, source.Symbol, dest.Symbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        return ComplexToComplex(source, dest, convertToInfo);
    }
    /// <summary>
    /// 转化为复杂类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? OtherToComplex(ITypeSymbolInfo source, ComplexTypeInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        if (sourceSymbol is INamedTypeSymbol namedType)
        {
            (_, converter) = GetConverter(_compilation, namedType, destSymbol);
            if (converter is not null)
                return CheckSource(converter, source.IsNullable, dest);
        }
        var constructors = SymbolReflection.GetConstructors(destSymbol);
        return ConstructorBySingle(constructors, sourceSymbol, dest) ??
            ConstructorByCompatibleParameter(constructors, sourceSymbol, dest);
    }
    /// <summary>
    /// 复杂类型转化为复杂类型
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public ISyntaxConverter? ComplexToComplex(ComplexTypeInfo sourceInfo, ComplexTypeInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        if (!convertToInfo.IsPartial)
            return null;
        var parameters = SymbolMember.GetTargetMembers(_typeBuilder, destInfo.Symbol, true);
        if (parameters.Count == 0)
            return null;
        var converter = Save(sourceInfo, destInfo, convertToInfo);
        var arguments = Map(_typeBuilder, parameters.Values, sourceInfo.Symbol).ToArray();
        var typeInfo = convertToInfo.TypeInfo;
        var symbolName = convertToInfo.Provider.SymbolName;
        var methodName = convertToInfo.MethodInfo.Name;

        var source = new ComplexSource(this, symbolName, destInfo, methodName, arguments);
        AddSource(source, typeInfo, sourceInfo.Symbol);
        return CheckSource(converter, sourceInfo.IsNullable, destInfo);
    }
    #endregion
    #region FromComplex
    /// <summary>
    /// 从复杂类型转化
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ComplexToOther(ComplexTypeInfo source, ITypeSymbolInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        ISyntaxConverter? converter;
        if (destSymbol is INamedTypeSymbol namedType)
        {
            (_, converter) = GetConverter(_compilation, sourceSymbol, namedType);
            if (converter is not null)
                return CheckSource(converter, source.IsNullable, dest);
        }
        converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        foreach (var item in SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol))
        {
            var itemType = item.Type;
            if (itemType.Equals(destSymbol, SymbolEqualityComparer.Default))
            {
                var itemInfo = _typeBuilder.Get(itemType);
                if (itemInfo is null)
                    continue;
                var memberConverter = new MemberConverter(item.Name);
                return CheckSource(memberConverter, itemInfo.IsNullable, dest);
            }
        }
        foreach (var item in SymbolReflection.GetPublicFieldsWithBase(sourceSymbol))
        {
            var itemType = item.Type;
            if (itemType.Equals(destSymbol, SymbolEqualityComparer.Default))
            {
                var itemInfo = _typeBuilder.Get(itemType);
                if (itemInfo is null)
                    continue;
                var memberConverter = new MemberConverter(item.Name);
                return CheckSource(memberConverter, itemInfo.IsNullable, dest);
            }
        }
        return null;
    }
    #endregion
    ///// <summary>
    ///// 从复杂类型转化
    ///// </summary>
    ///// <param name="sourceInfo"></param>
    ///// <param name="destSymbol"></param>
    ///// <param name="destInfo"></param>
    ///// <returns></returns>
    //public IConverter? FromComplex(ComplexTypeInfo sourceInfo, INamedTypeSymbol destSymbol, ITypeSymbolInfo destInfo)
    //{
    //    var sourceSymbol = sourceInfo.Symbol;
    //    //if (!convertToInfo.IsPartial)
    //    //    return null;
    //    foreach (var item in SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol))
    //    {
    //        if (item.Type is INamedTypeSymbol itemType && itemType.Equals(destSymbol, SymbolEqualityComparer.Default))
    //        {
    //            var itemInfo = _typeCacher.Get(itemType);
    //            if (itemInfo is null)
    //                continue;
    //            var memberConverter = new MemberConverter(item.Name);
    //            return CheckSource(memberConverter, itemInfo.IsNullable, destInfo);
    //        }
    //    }
    //    foreach (var item in SymbolReflection.GetPublicFieldsWithBase(sourceSymbol))
    //    {
    //        if (item.Type is INamedTypeSymbol itemType && itemType.Equals(destSymbol, SymbolEqualityComparer.Default))
    //        {
    //            var itemInfo = _typeCacher.Get(itemType);
    //            if (itemInfo is null)
    //                continue;
    //            var memberConverter = new MemberConverter(item.Name);
    //            return CheckSource(memberConverter, itemInfo.IsNullable, destInfo);
    //        }
    //    }
    //    return null;
    //}
    ///// <summary>
    ///// 转化为复杂类型
    ///// </summary>
    ///// <param name="sourceSymbol"></param>
    ///// <param name="destInfo"></param>
    ///// <param name="convertToInfo"></param>
    ///// <returns></returns>
    //public IConverter? ToComplex(ITypeSymbol sourceSymbol, ComplexTypeInfo destInfo, ConvertSourceInfo convertToInfo)
    //{
    //    //var sourceProvider = SourceProvider.Create(_compilation, sourceSymbol);
    //    //var convertToInfo = sourceProvider.ConvertTo(ComplexSymbol);
    //    //var methodInfo = convertToInfo.MethodInfo;
    //    //var convertToMethod = sourceProvider.GetConvertMethod(methodInfo, ComplexSymbol);
    //    //var typeInfo = convertToInfo.TypeInfo;
    //    //if (convertToMethod is not null)
    //    //    return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
    //    if (!convertToInfo.IsPartial)
    //        return null;
    //    var destSymbo = destInfo.Symbol;

    //    var constructors = SymbolReflection.GetConstructors(destSymbo);
    //    return ConstructorBySingle(constructors, sourceSymbol, destSymbo) ??
    //        ConstructorByCompatibleParameter(constructors, sourceSymbol, destSymbo);
    //}
    /// <summary>
    /// 尝试单参数构造函数
    /// </summary>
    /// <param name="sourceSymbol"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public static ISyntaxConverter? ConstructorBySingle(ITypeSymbol sourceSymbol, ComplexTypeInfo dest)
        => ConstructorBySingle(SymbolReflection.GetConstructors(dest.Symbol), sourceSymbol, dest);
    /// <summary>
    /// 尝试单参数构造函数
    /// </summary>
    /// <param name="constructors"></param>
    /// <param name="sourceSymbol"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public static ISyntaxConverter? ConstructorBySingle(IEnumerable<IMethodSymbol> constructors, ITypeSymbol sourceSymbol, ComplexTypeInfo dest)
    {
        var constructor = constructors.Where(m => SymbolReflection.MatchSingle(m.Parameters, sourceSymbol))
            .OrderBy(m => m.Parameters.Length)
            .FirstOrDefault();
        if (constructor is not null)
            return new ConstructorConverter(dest);
        return null;
    }
    /// <summary>
    /// 尝试兼容类型的参数
    /// </summary>
    /// <param name="constructors"></param>
    /// <param name="sourceSymbol"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ConstructorByCompatibleParameter(IEnumerable<IMethodSymbol> constructors, ITypeSymbol sourceSymbol, ComplexTypeInfo dest)
    {
        foreach (var item in constructors.Where(m => m.Parameters.Length == 1))
        {
            var parameter = item.Parameters[0];
            if (parameter.Type is not INamedTypeSymbol parameterType)
                continue;
            var parameterInfo = _typeBuilder.Get(parameterType);
            if (parameterInfo is null)
                continue;
            var parameterSymbol = parameterInfo.Symbol;
            // 尝试曲线救国
            // 尝试source转化为构造函数参数
            var compatibleConverter = Get(sourceSymbol, parameterSymbol);
            if (compatibleConverter is null)
                return null;
            var constructorConverter = new ConstructorConverter(dest);
            var converter = new CompositeConverter(compatibleConverter, constructorConverter);
            return converter;
        }
        return null;
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="members"></param>
    /// <param name="sourceSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<MemberArgument> Map(TypeInfoBuilder typeSymbols, IEnumerable<IMemberInfo> members, INamedTypeSymbol sourceSymbol)
    {
        var sourceMembers = SymbolMember.GetSourceMembers(typeSymbols, sourceSymbol);
        return Map(members, sourceMembers);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="members">成员</param>
    /// <param name="sourceMembers">来源</param>
    /// <returns></returns>
    public static IEnumerable<MemberArgument> Map(IEnumerable<IMemberInfo> members, IDictionary<string, IMemberInfo> sourceMembers)
    {
        foreach (var member in members)
        {
            if (sourceMembers.TryGetValue(member.Name, out var source))
                yield return new MemberArgument(member, source);
            // 构造函数参数即使匹配不上也要保留
            else if (member.IsParameter())
                yield return new MemberArgument(member, null);

        }
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="members">成员</param>
    /// <param name="sourceMembers">来源</param>
    /// <param name="references">参考规则</param>
    /// <returns></returns>
    public static List<MemberArgument> Map(IEnumerable<IMemberInfo> members, IDictionary<string, IMemberInfo> sourceMembers, List<MemberArgument> references)
    {
        var arguments = Map(members, sourceMembers)
            .ToList();
        return Reference(arguments, references);
    }
    /// <summary>
    /// 可反转的映射
    /// </summary>
    /// <param name="sourceMembers"></param>
    /// <param name="targetMembers"></param>
    /// <param name="publicMembers"></param>
    /// <returns></returns>
    public static IEnumerable<MemberArgument> ReversedMap(IDictionary<string, IMemberInfo> sourceMembers, IEnumerable<IMemberInfo> targetMembers, IDictionary<string, IMemberInfo> publicMembers)
    {
        foreach (var member in targetMembers)
        {
            var name = member.Name;
            sourceMembers.TryGetValue(name, out var source);

            if ((member.IsParameter() || !member.IsPublic)
                && publicMembers.TryGetValue(name, out var publicMember))
            {
                var reversedArgument = new MemberArgument(source, publicMember);
                // 源成员映射到参数,需要手动反转为属性映射源成员
                yield return new MemberReversedArgument(member, source, reversedArgument);
            }
            else
            {
                yield return new MemberArgument(member, source);
            }
        }
    }
    /// <summary>
    /// 参考
    /// </summary>
    /// <param name="arguments">成员映射</param>
    /// <param name="references">参考规则</param>
    /// <returns></returns>
    public static List<MemberArgument> Reference(List<MemberArgument> arguments, List<MemberArgument> references)
    {
        foreach (var referenced in references)
        {
            var referenceSource = referenced.Source;
            if (referenceSource is null)
                continue;
            var referenceMember = referenced.Member;
            var argument = arguments.FirstOrDefault(item => item.Member.Equals(referenceMember));
            if (argument is null)
            {
                arguments.Add(referenced);
            }
            else
            {
                argument.Source = referenceSource;
            }
        }
        return arguments;
    }
    ///// <summary>
    ///// 映射
    ///// </summary>
    ///// <param name="typeSymbols"></param>
    ///// <param name="destSymbol"></param>
    ///// <param name="sourceSymbol"></param>
    ///// <returns></returns>
    //public static IEnumerable<MemberArgument> Map(TypeSymbolCacher typeSymbols, INamedTypeSymbol destSymbol, INamedTypeSymbol sourceSymbol)
    //{
    //    var parameters = SymbolMember.GetTargetMembers(typeSymbols, destSymbol, true);
    //    if (parameters.Count == 0)
    //        return [];
    //    return Map(typeSymbols, parameters.Values, sourceSymbol);
    //}
}
