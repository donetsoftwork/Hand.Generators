using Hand.Builders;
using Hand.Enumerations.Services;
using Hand.Naming;
using Hand.Sources;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.GenerateEnumeration.Sources;

/// <summary>
/// 按枚举类型生成枚举类
/// </summary>
public class EnumSource(SyntaxGenerator generator, INamedTypeSymbol enumerationSymbol, InheritProvider naming, EnumService @enum, EnumerationProviderService provider, ISyntaxDisplay<TypeSyntax> providerType)
    : GeneratorSourceBase
{
    #region 配置
    /// <summary>
    /// 构造器
    /// </summary>
    protected readonly SyntaxGenerator _generator = generator;
    private readonly INamedTypeSymbol _enumerationSymbol = enumerationSymbol;
    private readonly InheritProvider _naming = naming;
    /// <inheritdoc cref="EnumService" path="/summary" />
    protected readonly EnumService _enum = @enum;
    private readonly EnumerationProviderService _provider = provider;
    private readonly ISyntaxDisplay<TypeSyntax> _providerType = providerType;

    /// <inheritdoc />
    public override string GenerateFileName 
        => $"{_enumerationSymbol.ToDisplayString()}.g.cs";
    #endregion

    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        var enumerationName = _enumerationSymbol.Name;
        //// 当前类及基类成员
        //var naming = new InheritProvider(_enumerationNaming.Typed(enumerationName), _baseNaming);
        var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
        // 如果没构造函数就生成构造函数
        if (!_naming.HasConstructor())
            GenerateConstructor(_generator, enumerationType.Identifier);
        // 生成枚举成员
        var (items, originalNames, aliasNames) = GenerateMembers(_naming);
        
        // 生成提供器
        #region GenerateProvider
        const string providerInstanceName = "Provider";
        if(_naming.TryDeclareField(providerInstanceName, out var isNew))
        {
            // 生成提供器实例字段
            var displayType = _providerType.Display(_generator);
            var providerInstanceField = _provider.GenerateInstance(displayType, providerInstanceName, isNew)
                .WithSummary("提供器");
            // 放最后
            _generator.AddOthers(providerInstanceField);
        }
        var providerType = _provider.ProviderType;
        var providerTypeDeclaration = SyntaxFactory.ClassDeclaration(providerType.Identifier)
            .Private()
            .WithSummary("提供器");
        var providerGenerator = SyntaxGenerator.Create(_generator, providerTypeDeclaration);
        GenerateProviderBase(providerGenerator, enumerationType);
        var providerConstructor = GenerateProviderConstructor(providerType.Identifier, enumerationType, items, originalNames, aliasNames);
        providerGenerator.AddConstructor(providerConstructor);
        var providerNaming = new TypedProvider(providerType.Identifier.ValueText).Inherit(new KindContainer());
        _provider.GenerateMembers(providerGenerator, providerNaming);
        #endregion
        return _generator;
    }
    /// <summary>
    /// 生成提供器基类
    /// </summary>
    /// <param name="providerGenerator"></param>
    /// <param name="enumerationType"></param>
    protected virtual void GenerateProviderBase(SyntaxGenerator providerGenerator, IdentifierNameSyntax enumerationType)
    {
        var providerBaseType = new GenericTypeNameInfo("EnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
        providerGenerator.AddBaseType(providerBaseType.Display(providerGenerator));
    }
    /// <summary>
    /// 生成枚举字段
    /// </summary>
    /// <param name="naming"></param>
    /// <returns></returns>
    public virtual (string[], Dictionary<long, string>, Dictionary<string, string>) GenerateMembers(InheritProvider naming)
        => _enum.GenerateMembers(_generator, naming);
    /// <summary>
    /// 生成枚举类构造函数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationName"></param>
    public virtual void GenerateConstructor(SyntaxGenerator generator, SyntaxToken enumerationName)
    {
        var name = SyntaxFactory.IdentifierName("name");
        var original = SyntaxFactory.IdentifierName("original");
        var description = SyntaxFactory.IdentifierName("description");
        var constructor = SyntaxFactory.ConstructorDeclaration(enumerationName)
            .AddParameterListParameters(
                SyntaxGenerator.StringType.Parameter(name.Identifier),
                SyntaxGenerator.LongType.Parameter(original.Identifier),
                SyntaxGenerator.StringType.Parameter(description.Identifier)
            )
            .WithBaseInitializer(name, original, description)
            .Private()
            .WithBody(SyntaxFactory.Block())
            .WithSummary("构造函数");
        generator.AddConstructor(constructor);
    }
    /// <summary>
    /// 生成枚举提供器构造函数
    /// </summary>
    /// <param name="providerName"></param>
    /// <param name="enumerationType"></param>
    /// <param name="items"></param>
    /// <param name="originalNames"></param>
    /// <param name="aliasNames"></param>
    /// <returns></returns>
    public virtual ConstructorDeclarationSyntax GenerateProviderConstructor(SyntaxToken providerName, TypeSyntax enumerationType, string[] items, Dictionary<long, string> originalNames, Dictionary<string, string> aliasNames)
    {
        var itemCollection = BuildItemCollection(enumerationType, items);
        var nameDictionary = BuildNameDictionary(_generator, enumerationType, items, aliasNames);
        var originalDictionary = BuildOriginalDictionary(_generator, enumerationType, originalNames);
        return SyntaxFactory.ConstructorDeclaration(providerName)
            .WithBaseInitializer(itemCollection, nameDictionary, originalDictionary)
            .WithBody(SyntaxFactory.Block())
            .Internal()
             .WithSummary("构造函数");
    }
    /// <summary>
    /// 构造枚举集合
    /// </summary>
    /// <param name="enumerationType"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static ExpressionSyntax BuildItemCollection(TypeSyntax enumerationType, string[] items)
        => SyntaxGenerator.Collection(Array.ConvertAll(items, item => enumerationType.Access(SyntaxFactory.IdentifierName(item))));
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
            builder.Add(SyntaxGenerator.Literal(name), enumerationType.Access(SyntaxFactory.IdentifierName(name)));
        foreach (var item in aliasNames)
            builder.Add(SyntaxGenerator.Literal(item.Key), enumerationType.Access(SyntaxFactory.IdentifierName(item.Value)));
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
        var dictionaryType = GenericTypeNameInfo.Dictionary("Dictionary", SyntaxGenerator.LongType, enumerationType);
        var originals = originalNames.Keys.OrderBy(original => original).ToArray();
        var builder = new DictionaryBuilder();
        foreach (var original in originals)
            builder.Add(SyntaxGenerator.Literal(original), enumerationType.Access(SyntaxFactory.IdentifierName(originalNames[original])));
        return builder.Build(dictionaryType.Display(generator));
    }
    /// <summary>
    /// 创建枚举类型提供器服务
    /// </summary>
    /// <param name="enumSymbol">枚举类型</param>
    /// <returns></returns>
    public static EnumerationProviderService Create(INamedTypeSymbol enumSymbol)
    {
        var enumerationName = enumSymbol.Name;
        var enumerationType = SyntaxFactory.IdentifierName("Enumeration");
        var providerType = SyntaxFactory.IdentifierName(enumerationName + "Provider");
        //var baseType = new GenericTypeNameInfo("EnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
        return new EnumerationProviderService(enumerationType, providerType);
    }
}
