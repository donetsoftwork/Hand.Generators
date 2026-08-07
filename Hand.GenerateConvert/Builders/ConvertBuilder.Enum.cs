using Hand.Converters;
using Hand.Enums;
using Hand.Members;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 枚举转化构造器
/// </summary>
public partial class ConvertBuilder
{
    /// <summary>
    /// Enum转其他类型
    /// </summary>
    /// <param name="enumSymbol"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public IConverter? FromEnum(INamedTypeSymbol enumSymbol, TypeSymbolInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        var enumBundle = _bundles.Get(enumSymbol);
        var underType = enumBundle.UnderType;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, destType))
        //    return new CastConverter(destType.ToSyntax());

        // 枚举转string
        // 正常情况下该逻辑应该不会命中,之前ConvertBuilder已经处理ToString了
        //if (destType.IsString())
        //    return ToStringConverter.Instance;
        var destSymbol = destInfo.Symbol;
        if (destInfo.Kind == TypeSymbolKind.Enum)
        {
            var destBundle = _bundles.Get(destSymbol);
            // 枚举转枚举
            if (destBundle is not null)
                return EnumToEnum(enumSymbol, destInfo, enumBundle, destBundle, convertToInfo);
        }
        var original = Get(underType, destSymbol);
        if (original is null)
            return null;
        // 先转为底层类型, 再转目标类型
        return new CompatibleConverter(new CastConverter(underType.ToSyntax()), original);
    }
    /// <summary>
    ///其他类型转Enum
    /// </summary>
    /// <param name="sourceSymbol"></param>
    /// <param name="enumInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public IConverter? ToEnum(INamedTypeSymbol sourceSymbol, TypeSymbolInfo enumInfo, ConvertSourceInfo convertToInfo)
    {
        var enumSymbol = enumInfo.Symbol;
        var enumBundle = _bundles.Get(enumSymbol);
        // string转枚举
        if (sourceSymbol.IsString())
            return EnumFromString(enumInfo, enumBundle, convertToInfo);
        var underType = enumBundle.UnderType;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, sourceType))
        //    return new CastConverter(enumType.ToSyntax());

        // 使用Enum.ToObject转化,支持整数类型
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (EnumToObjectConverter.CheckSupported(sourceType.SpecialType))
        //    return new EnumToObjectConverter(enumType.ToSyntax());

        var compatible = Get(sourceSymbol, underType);
        if (compatible is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        return new CompatibleConverter(compatible, new CastConverter(enumSymbol.ToSyntax()));
    }
    /// <summary>
    /// string转Enum
    /// </summary>
    /// <param name="enumInfo"></param>
    /// <param name="bundle"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    private IConverter EnumFromString(TypeSymbolInfo enumInfo, IEnumBundle bundle, ConvertSourceInfo convertToInfo)
    {
        // 枚举特殊成员
        var members = bundle.Fields
            .Where(field => !string.IsNullOrWhiteSpace(field.Member))
            .ToArray();
        var enumSymbol = enumInfo.Symbol;
        // 默认通过Enum.Parse转化
        if (members.Length == 0)
            return new EnumParseConverter(enumSymbol.ToSyntax(), true);

        //var enumProvider = SourceProvider.CreateByExtension(compilation, enumType);
        //var convertFromInfo = enumProvider.ConvertFrom(stringType.Name);
        //var convertFromMethod = enumProvider.GetConvertMethod(convertFromInfo.MethodInfo, stringType);
        //if(convertFromMethod is null)
        //{
        //    var stringProvider = SourceProvider.CreateByExtension(compilation, stringType);
        //    var convertToInfo = stringProvider.ConvertTo(enumType.Name);
        //    var convertToMethod = stringProvider.GetConvertMethod(convertToInfo.MethodInfo, enumType);
        //}

        //var stringProvider = SourceProvider.CreateByExtension(_compilation, stringType);
        //var convertToInfo = stringProvider.ConvertTo(enumType.Name);
        //var methodInfo = convertToInfo.MethodInfo;
        //var convertToMethod = stringProvider.GetConvertMethod(methodInfo, enumType);
        //var typeInfo = convertToInfo.TypeInfo;
        //if (convertToMethod is not null)
        //    return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        // 扩展类非partial,无法扩展,直接使用Enum.Parse转化
        if (!convertToInfo.IsPartial)
            return new EnumParseConverter(enumSymbol.ToSyntax(), true);
        
        var typeInfo = convertToInfo.TypeInfo;
        var methodName = convertToInfo.MethodInfo.Name;
        if(bundle.HasFlag && bundle is FlagEnumBundle flagBundle)
        {
            var source = new FlagEnumFromMemberStringSource(_compilation, enumInfo, methodName, members);
            AddSource(source, typeInfo);
        }
        else
        {
            var source = new EnumFromMemberStringSource(_compilation, enumInfo, methodName, members);
            AddSource(source, typeInfo);
        }
        return new StaticMethodConverter(typeInfo.Type.Access(methodName));
    }
    private IConverter? EnumToEnum(INamedTypeSymbol sourceSymbol, TypeSymbolInfo destInfo, ConvertSourceInfo convertToInfo)
        => EnumToEnum(sourceSymbol, destInfo, _bundles.Get(sourceSymbol), _bundles.Get(destInfo.Symbol), convertToInfo);
    private IConverter? EnumToEnum(INamedTypeSymbol sourceSymbol, TypeSymbolInfo destInfo, IEnumBundle sourceBundle, IEnumBundle destBundle, ConvertSourceInfo convertToInfo)
    {
        //var methodInfo = convertToInfo.MethodInfo;        
        //if (convertToMethod is not null)
        //    return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        // 扩展类非partial,无法扩展,不支持
        if (!convertToInfo.IsPartial)
            return null;

        var typeInfo = convertToInfo.TypeInfo;
        var methodName = convertToInfo.MethodInfo.Name;
        var source = new EnumToEnumSource(_compilation, SyntaxFactory.IdentifierName(sourceSymbol.Name), destInfo, methodName, sourceBundle, destBundle);
        AddSource(source, typeInfo);
        return new StaticMethodConverter(typeInfo.Type.Access(methodName));
    }
}
