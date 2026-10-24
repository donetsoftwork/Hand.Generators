using Hand.Enumerations;
using Hand.Enumerations.Services;
using Hand.Naming;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.GenerateEnumeration.Sources;

/// <summary>
/// 按位标记枚举类型生成枚举类
/// </summary>
public class FlagEnumSource(SyntaxGenerator generator, INamedTypeSymbol enumerationSymbol, InheritProvider naming, EnumService @enum, FlagEnumerationProviderService provider, ISyntaxDisplay<TypeSyntax> providerType, string emptyName)
    : EnumSource(generator, enumerationSymbol, naming, @enum, provider, providerType)
{
    private readonly string _emptyName = emptyName;

    /// <inheritdoc />
    public override (string[], Dictionary<long, string>, Dictionary<string, string>) GenerateMembers(InheritProvider naming)
    {
        var (items, originalNames, aliasNames) = base.GenerateMembers(naming);
        if (naming.TryDeclareField(_emptyName, out var isNew))
        {
            var original = 0L;
             _enum.GenerateField(_generator, _emptyName, original, "逻辑空", isNew);
            originalNames[original] = _emptyName;
            return ([_emptyName, .. items], originalNames, aliasNames);
        }
        return (items, originalNames, aliasNames);
    }
    /// <inheritdoc />
    public override void GenerateConstructor(SyntaxGenerator generator, SyntaxToken enumerationName)
    {
        var stringType = SyntaxGenerator.StringType;
        var flagsType = GenericTypeNameInfo.Collection("ISet", stringType);
        var flags = SyntaxFactory.IdentifierName("flags");
        var original = SyntaxFactory.IdentifierName("original");
        var description = SyntaxFactory.IdentifierName("description");
        var constructor = SyntaxFactory.ConstructorDeclaration(enumerationName)
            .AddParameterListParameters(
                flagsType.Display(generator).Parameter(flags.Identifier),
                SyntaxGenerator.LongType.Parameter(original.Identifier),
                stringType.Parameter(description.Identifier)
            )
            .WithBaseInitializer(flags, original, description)
            .Private()
            .WithBody(SyntaxFactory.Block())
            .WithSummary("构造函数");
        generator.AddConstructor(constructor);
        base.GenerateConstructor(generator, enumerationName);
    }
    /// <inheritdoc />
    protected override void GenerateProviderBase(SyntaxGenerator providerGenerator, IdentifierNameSyntax enumerationType)
    {
        var providerBaseType = new GenericTypeNameInfo("FlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
        providerGenerator.AddBaseType(providerBaseType.Display(providerGenerator));
    }
    /// <inheritdoc />
    public override ConstructorDeclarationSyntax GenerateProviderConstructor(SyntaxToken providerName, TypeSyntax enumerationType, string[] items, Dictionary<long, string> originalNames, Dictionary<string, string> aliasNames)
    {
        var itemCollection = BuildItemCollection(enumerationType, items);
        var flagsCollection = BuildItemCollection(enumerationType, [.. GetFlags(originalNames).Distinct()]);
        var nameDictionary = BuildNameDictionary(_generator, enumerationType, items, aliasNames);
        var originalDictionary = BuildOriginalDictionary(_generator, enumerationType, originalNames);
        return SyntaxFactory.ConstructorDeclaration(providerName)
            .WithBaseInitializer(itemCollection, flagsCollection, nameDictionary, originalDictionary)
            .WithBody(SyntaxFactory.Block())
            .Internal()
            .WithSummary("构造函数");
    }
    /// <summary>
    /// 获取位标记枚举名
    /// </summary>
    /// <param name="originalNames"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetFlags(Dictionary<long, string> originalNames)
    {
        foreach (var item in originalNames)
        {
            if (VerifyFlag(item.Key))
                yield return item.Value;
        }
    }
    /// <summary>
    /// 检查位标记
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static bool VerifyFlag(long original)
    {
        if (original < 1L)
            return false;
        // 2的n次
        return (original & (original - 1L)) == 0L;
    }
}
