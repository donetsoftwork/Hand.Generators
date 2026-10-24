using Hand.Builders;
using Hand.Enumerations.Services;
using Hand.Enums.Builders;
using Hand.Members;
using Hand.Naming;
using Hand.Reflection;
using Hand.Sources;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand.Enumerations.Sources;

/// <summary>
/// 枚举类源
/// </summary>
public class EnumProviderSource(SyntaxGenerator generator, NameSyntax @namespace, string providerName, string enumerationName, EnumService @enum, EnumerationProviderService provider, INamingProvider baseNaming)
    : GeneratorSourceBase
{
    #region 配置
    private readonly SyntaxGenerator _generator = generator;
    private readonly NameSyntax _namespace = @namespace;
    private readonly string _providerName = providerName;
    private readonly string _enumerationName = enumerationName;
    private readonly EnumService _enum = @enum;
    private readonly EnumerationProviderService _provider = provider;
    private readonly INamingProvider _baseNaming = baseNaming;
    /// <summary>
    /// 实例名
    /// </summary>
    public const string InstanceName = "Instance";

    /// <inheritdoc />
    public override string GenerateFileName => _namespace + "." + _providerName + ".g.cs";
    #endregion

    /// <summary>
    /// 获取实例成员
    /// </summary>
    /// <returns></returns>
    public StaticMember GetInstanceMember()
        => new(_namespace, _providerName, SyntaxFactory.IdentifierName(InstanceName));

    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        //var declaration = SyntaxFactory.ClassDeclaration(_providerName)
        //    .Public();
        //var generator = SyntaxGenerator.Create(_namespace, declaration);
        // 当前类及基类成员
        var naming = new InheritProvider(new TypedProvider(_providerName), _baseNaming);
        var (items, originalNames, aliasNames) = _enum.GenerateMembers(_generator, naming);
        var enumerationType = SyntaxFactory.IdentifierName(_enumerationName);
        var constructor = GenerateConstructor(_generator, enumerationType, items, originalNames, aliasNames);
        _generator.AddConstructor(constructor);
        // 生成实例字段
        var instanceField = _provider.GenerateInstance(InstanceName);
        // 放最后
        _generator.AddOthers(instanceField);
        return _generator;
    }
    /// <summary>
    /// 生成构造函数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationType"></param>
    /// <param name="items"></param>
    /// <param name="originalNames"></param>
    /// <param name="aliasNames"></param>
    /// <returns></returns>
    public static ConstructorDeclarationSyntax GenerateConstructor(SyntaxGenerator generator, TypeSyntax enumerationType, string[] items, Dictionary<long, string> originalNames, Dictionary<string, string> aliasNames)
    {
        var itemCollection = BuildItemCollection(items);
        var nameDictionary = BuildNameDictionary(generator, enumerationType, items, aliasNames);
        var originalDictionary = BuildOriginalDictionary(generator, enumerationType, originalNames);
        return generator.Declaration.Constructor()
            .WithBaseInitializer(itemCollection, nameDictionary, originalDictionary)
            .WithBody(SyntaxFactory.Block())
            .Internal();
    }
    /// <summary>
    /// 构造枚举集合
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static ExpressionSyntax BuildItemCollection(string[] items)
        => SyntaxGenerator.Collection(Array.ConvertAll(items, SyntaxFactory.IdentifierName));
    /// <summary>
    /// 构造枚举名字典
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationType"></param>
    /// <param name="items"></param>
    /// <param name="aliasNames"></param>
    /// <returns></returns>
    public static ExpressionSyntax BuildNameDictionary(SyntaxGenerator generator, TypeSyntax enumerationType, string[] items, Dictionary<string, string> aliasNames)
    {
        var dictionaryType = GenericTypeNameInfo.Dictionary("Dictionary", SyntaxGenerator.StringType, enumerationType); 
        var builder = new DictionaryBuilder();
        foreach (var name in items)
            builder.Add(SyntaxGenerator.Literal(name), SyntaxFactory.IdentifierName(name));
        foreach(var item in aliasNames)
            builder.Add(SyntaxGenerator.Literal(item.Key), SyntaxFactory.IdentifierName(item.Value));
        return builder.Build(dictionaryType.Display(generator));
    }
    /// <summary>
    /// 构造原始值字典
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationType"></param>
    /// <param name="originalNames"></param>
    /// <returns></returns>
    public static ExpressionSyntax BuildOriginalDictionary(SyntaxGenerator generator, TypeSyntax enumerationType, Dictionary<long, string> originalNames)
    {
        var dictionaryType = GenericTypeNameInfo.Dictionary("Dictionary", SyntaxGenerator.StringType, enumerationType);
        var originals = originalNames.Keys.OrderBy(original => original).ToArray();
        var builder = new DictionaryBuilder();
        foreach (var original in originals)
            builder.Add(SyntaxGenerator.Literal(original), SyntaxFactory.IdentifierName(originalNames[original]));
        return builder.Build(dictionaryType.Display(generator));
    }
    /// <summary>
    /// 创建枚举类提供源
    /// </summary>
    /// <param name="enumSymbol"></param>
    /// <param name="bundleBuilder"></param>
    /// <param name="providerBaseNaming"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumProviderSource Create(INamedTypeSymbol enumSymbol, EnumBundleBuilder bundleBuilder, INamingProvider providerBaseNaming)
        => Create(enumSymbol, providerBaseNaming, new EnumService(bundleBuilder, SyntaxFactory.IdentifierName("Enumeration"), [.. SymbolReflection.GetEnumFields(enumSymbol)]), "Enumeration");
    ///// <summary>
    ///// 创建枚举类提供源
    ///// </summary>
    ///// <param name="enumSymbol"></param>
    ///// <param name="bundleBuilder"></param>
    ///// <param name="providerBaseNaming"></param>
    ///// <param name="enumerationName"></param>
    ///// <returns></returns>
    //private static EnumProviderSource Create(INamedTypeSymbol enumSymbol, EnumBundleBuilder bundleBuilder, INamingProvider providerBaseNaming, string enumerationName)
    //    => Create(enumSymbol, providerBaseNaming, new EnumService(bundleBuilder, SyntaxFactory.IdentifierName(enumerationName), [.. SymbolReflection.GetEnumFields(enumSymbol)]), enumerationName);
    //{
    //    var fields = SymbolReflection.GetEnumFields(enumSymbol).ToArray();
    //    var enumerationName = "Enumeration";
    //    var providerName = enumSymbol.Name + "Provider";
    //    var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
    //    var enumService = new EnumService(bundleBuilder, enumerationType, fields);
    //    return new(enumSymbol.ContainingNamespace.ToName(), providerName, enumerationName, enumService, EnumerationProviderService.Create(enumerationName, providerName), providerBaseNaming);
    //}
    /// <summary>
    /// 创建枚举类提供源
    /// </summary>
    /// <param name="enumSymbol"></param>
    /// <param name="providerBaseNaming"></param>
    /// <param name="enumService"></param>
    /// <param name="enumerationName"></param>
    /// <returns></returns>
    private static EnumProviderSource Create(INamedTypeSymbol enumSymbol, INamingProvider providerBaseNaming, EnumService enumService, string enumerationName)
    {
        var @namespace = enumSymbol.ContainingNamespace.ToName();
        var providerName = enumSymbol.Name + "Provider";
        var declaration = SyntaxFactory.ClassDeclaration(providerName)
            .Public();
        var generator = SyntaxGenerator.Create(@namespace, declaration);
        return new(generator, @namespace, providerName, enumerationName, enumService, EnumerationProviderService.Create(enumerationName, providerName), providerBaseNaming);
    }
    /// <summary>
    /// 创建枚举类提供源
    /// </summary>
    /// <param name="enumSymbol"></param>
    /// <param name="bundleBuilder"></param>
    /// <param name="providerBaseNaming"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumProviderSource CreateFlag(INamedTypeSymbol enumSymbol, EnumBundleBuilder bundleBuilder, INamingProvider providerBaseNaming)
        => Create(enumSymbol, providerBaseNaming, new FlagEnumProviderService(bundleBuilder, SyntaxFactory.IdentifierName("FlagEnumeration"), [.. SymbolReflection.GetEnumFields(enumSymbol)]), "FlagEnumeration");
    //{
    //    var fields = SymbolReflection.GetEnumFields(enumSymbol).ToArray();
    //    var enumerationName = "FlagEnumeration";
    //    var providerName = enumSymbol.Name + "Provider";
    //    var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
    //    var enumService = new FlagEnumProviderService(bundleBuilder, enumerationType, fields);
    //    return new(enumSymbol.ContainingNamespace.ToName(), providerName, enumerationName, enumService, EnumerationProviderService.Create(enumerationName, providerName), providerBaseNaming);
    //}
    ///// <summary>
    ///// 创建标记枚举类提供源
    ///// </summary>
    ///// <param name="enumerationSymbol"></param>
    ///// <param name="emptyMemberName"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static EnumProviderSource CreateFlag(INamedTypeSymbol enumerationSymbol, string emptyMemberName)
    //    => new(enumerationSymbol.ContainingNamespace.ToName(), enumerationSymbol.Name + "Provider", FlagEnumerationProviderService.Create(enumerationSymbol, emptyMemberName), [.. GetMembers(enumerationSymbol)]);
    ///// <summary>
    ///// 创建标记枚举类提供源
    ///// </summary>
    ///// <param name="enumerationSymbol"></param>
    ///// <returns></returns>
    //public static EnumProviderSource? CreateFlag(INamedTypeSymbol enumerationSymbol)
    //{
    //    var members = GetMembers(enumerationSymbol)
    //        .ToArray();
    //    if (members.Length == 0)
    //        return null;
    //    var service = FlagEnumerationProviderService.Create(enumerationSymbol, members[0]);
    //    return new(enumerationSymbol.ContainingNamespace.ToName(), enumerationSymbol.Name + "Provider", service, members);
    //}
}
