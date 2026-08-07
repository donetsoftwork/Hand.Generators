using Hand.Cachers;
using Hand.Converters;
using Hand.Members;
using Hand.Reflection;
using Hand.Sources;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 复杂类型转化
/// </summary>
public partial class ConvertBuilder
{
    /// <summary>
    /// 转化为实体
    /// </summary>
    /// <param name="sourceSymbol"></param>
    /// <param name="ComplexSymbol"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public IConverter? ToComplex(INamedTypeSymbol sourceSymbol, INamedTypeSymbol ComplexSymbol, ConvertSourceInfo convertToInfo)
    {
        //var sourceProvider = SourceProvider.Create(_compilation, sourceSymbol);
        //var convertToInfo = sourceProvider.ConvertTo(ComplexSymbol);
        //var methodInfo = convertToInfo.MethodInfo;
        //var convertToMethod = sourceProvider.GetConvertMethod(methodInfo, ComplexSymbol);
        //var typeInfo = convertToInfo.TypeInfo;
        //if (convertToMethod is not null)
        //    return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        if (!convertToInfo.IsPartial)
            return null;

        var constructors = SymbolReflection.GetConstructors(ComplexSymbol);
        return ToComplexBySingle(constructors, sourceSymbol, ComplexSymbol) ??
            ToComplexByCompatibleParameter(constructors, sourceSymbol, ComplexSymbol);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="constructors"></param>
    /// <param name="sourceSymbol"></param>
    /// <param name="ComplexSymbol"></param>
    /// <returns></returns>
    public IConverter? ToComplexBySingle(IEnumerable<IMethodSymbol> constructors, INamedTypeSymbol sourceSymbol, INamedTypeSymbol ComplexSymbol)
    {
        var constructor = constructors.Where(m => SymbolTypeDescriptor.MatchSingle(m.Parameters, sourceSymbol))
            .OrderBy(m => m.Parameters.Length)
            .FirstOrDefault();
        if (constructor is not null)
            return new ConstructorConverter(ComplexSymbol.ToSyntax());
        return null;
    }
    /// <summary>
    /// 尝试兼容的参数
    /// </summary>
    /// <param name="constructors"></param>
    /// <param name="sourceSymbol"></param>
    /// <param name="ComplexSymbol"></param>
    /// <returns></returns>
    public IConverter? ToComplexByCompatibleParameter(IEnumerable<IMethodSymbol> constructors, INamedTypeSymbol sourceSymbol, INamedTypeSymbol ComplexSymbol)
    {
        foreach (var item in constructors.Where(m => m.Parameters.Length == 1))
        {
            var parameter = item.Parameters[0];
            if (parameter.Type is not INamedTypeSymbol parameterType)
                continue;
            var parameterInfo = _typeCacher.Get(parameterType);
            if (parameterInfo is null)
                continue;
            var parameterSymbol = parameterInfo.Symbol;
            // 尝试曲线救国
            // 尝试source转化为构造函数参数
            var compatibleConverter = Get(sourceSymbol, parameterSymbol);
            if (compatibleConverter is null)
                return null;
            var constructorConverter = new ConstructorConverter(ComplexSymbol.ToSyntax());
            var converter = new CompatibleConverter(compatibleConverter, constructorConverter);
            return converter;
        }
        return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ComplexSymbol"></param>
    /// <param name="destSymbol"></param>
    /// <param name="destInfo"></param>
    /// <returns></returns>
    public IConverter? FromComplex(INamedTypeSymbol ComplexSymbol, INamedTypeSymbol destSymbol, TypeSymbolInfo destInfo)
    {
        //if (!convertToInfo.IsPartial)
        //    return null;
        foreach (var item in SymbolReflection.GetPublicPropertiesWithBase(ComplexSymbol))
        {
            if (item.Type is INamedTypeSymbol itemType && itemType.Equals(destSymbol, SymbolEqualityComparer.Default))
            {
                var itemInfo = _typeCacher.Get(itemType);
                if (itemInfo is null)
                    continue;
                var memberConverter = new MemberConverter(item.Name);
                return CheckSource(memberConverter, itemInfo.Kind.IsNullable(), destInfo);
            }
        }
        foreach (var item in SymbolReflection.GetPublicFieldsWithBase(ComplexSymbol))
        {
            if (item.Type is INamedTypeSymbol itemType && itemType.Equals(destSymbol, SymbolEqualityComparer.Default))
            {
                var itemInfo = _typeCacher.Get(itemType);
                if (itemInfo is null)
                    continue;
                var memberConverter = new MemberConverter(item.Name);
                return CheckSource(memberConverter, itemInfo.Kind.IsNullable(), destInfo);
            }
        }
        return null;
    }
    /// <summary>
    /// 实体转化为实体
    /// </summary>
    /// <param name="sourceSymbol"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public IConverter? ComplexToComplex(INamedTypeSymbol sourceSymbol, TypeSymbolInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        if (!convertToInfo.IsPartial)
            return null;
        var parameters = SymbolMember.GetTargetMembers(_typeCacher, destInfo.Symbol, true);
        if (parameters.Count == 0)
            return null;
        var sourceInfo = _typeCacher.Get(sourceSymbol)!;
        var converter = Save(sourceInfo, destInfo, convertToInfo);
        var arguments = Map(_typeCacher, parameters.Values, sourceSymbol).ToArray();
        var typeInfo = convertToInfo.TypeInfo;
        var symbolName = convertToInfo.Provider.SymbolName;
        var methodName = convertToInfo.MethodInfo.Name;

        var source = new ComplexSource(this, symbolName, destInfo, methodName, arguments);
        AddSource(source, typeInfo);
        return converter;
        //var constructors = SymbolReflection.GetConstructors(ComplexSymbol);
        //var converter = ToComplexBySingle(constructors, sourceSymbol, ComplexSymbol);
        //if (converter is not null)
        //    return converter;
        //var parameters = MemberParameter.GetParametersByType(_compilation, ComplexSymbol, true);
        //if (parameters.Count == 0)
        //    return null;

        //var arguments = Map(_compilation, parameters.Values, sourceSymbol).ToArray();
        //return new ComplexConverter(this, ComplexSymbol.ToSyntax(), arguments);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="destSymbol"></param>
    /// <param name="sourceSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<MemberArgument> Map(TypeSymbolCacher typeSymbols, INamedTypeSymbol destSymbol, INamedTypeSymbol sourceSymbol)
    {
        var parameters = SymbolMember.GetTargetMembers(typeSymbols, destSymbol, true);
        if (parameters.Count == 0)
            return [];
        return Map(typeSymbols, parameters.Values, sourceSymbol);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="parameters"></param>
    /// <param name="sourceSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<MemberArgument> Map(TypeSymbolCacher typeSymbols, IEnumerable<Member> parameters, INamedTypeSymbol sourceSymbol)
    {
        var sourceMembers = SymbolMember.GetSourceMembers(typeSymbols, sourceSymbol);
        foreach (var parameter in parameters)
        {
            var name = parameter.Name;
            sourceMembers.TryGetValue(name, out var sourceMember);
            yield return new MemberArgument(parameter, sourceMember);
        }
    }
}
