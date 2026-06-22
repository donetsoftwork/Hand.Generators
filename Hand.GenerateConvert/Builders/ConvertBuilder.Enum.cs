using Hand.Converters;
using Hand.Enums;
using Hand.Providers;
using Hand.Symbols;
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
    /// <param name="enumType"></param>
    /// <param name="destType"></param>
    /// <returns></returns>
    public IConverter? FromEnum(INamedTypeSymbol enumType, INamedTypeSymbol destType)
    {
        var enumBundle = _bundleBuilder.Get(enumType);
        var underType = enumBundle.UnderType;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, destType))
        //    return new CastConverter(destType.ToSyntax());

        // 枚举转string
        // 正常情况下该逻辑应该不会命中,之前ConvertBuilder已经处理ToString了
        //if (destType.IsString())
        //    return ToStringConverter.Instance;

        if (destType.IsEnum())
        {
            var destBundle = _bundleBuilder.Get(destType);
            // 枚举转枚举
            if (destBundle is not null)
                return EnumToEnum(enumType, destType, enumBundle, destBundle);
        }
        var original = Get(underType, destType);
        if (original is null)
            return null;
        // 先转为底层类型, 再转目标类型
        return new CompatibleConverter(new CastConverter(underType.ToSyntax()), original);
    }
    /// <summary>
    ///其他类型转Enum
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public IConverter? ToEnum(INamedTypeSymbol sourceType, INamedTypeSymbol enumType)
    {
        var enumBundle = _bundleBuilder.Get(enumType);
        // string转枚举
        if (sourceType.IsString())
            return EnumFromString(sourceType, enumType, enumBundle);
        var underType = enumBundle.UnderType;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, sourceType))
        //    return new CastConverter(enumType.ToSyntax());

        // 使用Enum.ToObject转化,支持整数类型
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (EnumToObjectConverter.CheckSupported(sourceType.SpecialType))
        //    return new EnumToObjectConverter(enumType.ToSyntax());

        var compatible = Get(sourceType, underType);
        if (compatible is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        return new CompatibleConverter(compatible, new CastConverter(enumType.ToSyntax()));
    }
    /// <summary>
    /// string转Enum
    /// </summary>
    /// <param name="stringType"></param>
    /// <param name="enumType"></param>
    /// <param name="bundle"></param>
    /// <returns></returns>
    private IConverter EnumFromString(INamedTypeSymbol stringType, INamedTypeSymbol enumType, IEnumBundle bundle)
    {
        // 枚举特殊成员
        var members = bundle.Fields
            .Where(field => !string.IsNullOrWhiteSpace(field.Member))
            .ToArray();
        // 默认通过Enum.Parse转化
        if (members.Length == 0)
            return new EnumParseConverter(enumType.ToSyntax(), true);

        //var enumProvider = SourceProvider.CreateByExtension(compilation, enumType);
        //var convertFromInfo = enumProvider.ConvertFrom(stringType.Name);
        //var convertFromMethod = enumProvider.GetConvertMethod(convertFromInfo.MethodInfo, stringType);
        //if(convertFromMethod is null)
        //{
        //    var stringProvider = SourceProvider.CreateByExtension(compilation, stringType);
        //    var convertToInfo = stringProvider.ConvertTo(enumType.Name);
        //    var convertToMethod = stringProvider.GetConvertMethod(convertToInfo.MethodInfo, enumType);
        //}

        var stringProvider = SourceProvider.CreateByExtension(_compilation, stringType);
        var convertToInfo = stringProvider.ConvertTo(enumType.Name);
        var methodInfo = convertToInfo.MethodInfo;
        var convertToMethod = stringProvider.GetConvertMethod(methodInfo, enumType);
        var typeInfo = convertToInfo.TypeInfo;
        if (convertToMethod is not null)
            return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        // 扩展类非partial,无法扩展,直接使用Enum.Parse转化
        if (!convertToInfo.IsPartial)
            return new EnumParseConverter(enumType.ToSyntax(), true);
        
        var methodName = methodInfo.Name;
        if(bundle.HasFlag && bundle is FlagEnumBundle flagBundle)
        {
            var source = new FlagEnumFromMemberStringSource(_compilation, enumType.ToSyntax(), typeInfo, methodName, [.. flagBundle.Fields]);
            AddSource(source);
        }
        else
        {
            var source = new EnumFromMemberStringSource(_compilation, enumType.ToSyntax(), typeInfo, methodName, members);
            AddSource(source);
        }
        return new StaticMethodConverter(typeInfo.Type.Access(methodName));
    }
    private IConverter? EnumToEnum(INamedTypeSymbol sourceType, INamedTypeSymbol destType)
        => EnumToEnum(sourceType, destType, _bundleBuilder.Get(sourceType), _bundleBuilder.Get(destType));
    private IConverter? EnumToEnum(INamedTypeSymbol sourceType, INamedTypeSymbol destType, IEnumBundle sourceBundle, IEnumBundle destBundle)
    {
        var sourceProvider = SourceProvider.CreateByExtension(_compilation, sourceType);
        var convertToInfo = sourceProvider.ConvertTo(destType.Name);
        var methodInfo = convertToInfo.MethodInfo;
        var convertToMethod = sourceProvider.GetConvertMethod(methodInfo, destType);
        var typeInfo = convertToInfo.TypeInfo;
        if (convertToMethod is not null)
            return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        // 扩展类非partial,无法扩展,不支持
        if (!convertToInfo.IsPartial)
            return null;

        
        var methodName = methodInfo.Name;
        var source = new EnumToEnumSource(_compilation, SyntaxFactory.IdentifierName(sourceType.Name), destType.ToSyntax(), typeInfo, methodName, sourceBundle, destBundle);
        AddSource(source);
        return new StaticMethodConverter(typeInfo.Type.Access(methodName));
    }
}
