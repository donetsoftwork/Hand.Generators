using Hand.Maping;
using Hand.Rule;
using Hand.Sources;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace Hand.GeneratePoco;

/// <summary>
/// Poco生成源
/// </summary>
public class PocoSource(TypeDeclarationSyntax type, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol fromSymbol, IRecognizer<string>[] propertyRules, IValidation<string>? nullableRule, bool init)
    : IGeneratorSource
{
    #region 配置
    private readonly TypeDeclarationSyntax _type = type;
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    //private readonly INamedTypeSymbol _fromSymbol = fromSymbol;
    private readonly IDictionary<string, IPropertySymbol> _properties = GetProperties(fromSymbol, propertyRules);
    private readonly IValidation<string>? _nullableRule = nullableRule;
    private readonly bool _init = init;

    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    /// <summary>
    /// 反射信息
    /// </summary>
    public INamedTypeSymbol Symbol
        => _typeSymbol;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_typeSymbol.ToDisplayString()}.Poco.g.cs";
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var properties0 = SymbolReflection.GetProperties(_typeSymbol)
            .Select(p => p.Name)
            .ToFrozenSet();
        var kinds = ChecAccessorKinds(_init);
        List<string> namespaces = [];
        foreach (var item in _properties)
        {
            var name = item.Key;            
            if (properties0.Contains(name))
                continue;            
            var propertySymbol = item.Value;
            var propertySymbolType = CheckPropertySymbol(_compilation, propertySymbol.Type);
            if (propertySymbolType is null)
                continue;
            var attributes = propertySymbol.GetAttributes().ToSyntax(namespaces);
            var propertyType = CheckPropertyNullAble(name, propertySymbolType.ToSyntax());
            var property = propertyType.Property(name, kinds)
                .AddAttributeLists(attributes)
                .Public();
            builder.AddMember(property);
        }
        builder.Using(namespaces);
        return builder;
    }
    /// <summary>
    /// 属性操作器种类
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public static SyntaxKind[] ChecAccessorKinds(bool init)
    {
        if (init)
            return [SyntaxKind.GetAccessorDeclaration, SyntaxKind.InitAccessorDeclaration];
        return [SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration];
    }
    /// <summary>
    /// 判断属性是否可空
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    public TypeSyntax CheckPropertyNullAble(string propertyName, TypeSyntax propertyType)
    {
        if (_nullableRule is null)
            return propertyType;        
        if (_nullableRule.Validate(propertyName))
            return propertyType.CheckNullable();

        return propertyType;
    }
    /// <summary>
    /// 获取原始类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? CheckPropertySymbol(Compilation compilation, ITypeSymbol symbol)
    {
        if(symbol is INamedTypeSymbol namedTypeSymbol)
        {
            var interfaces = namedTypeSymbol.AllInterfaces;
            var entityId = compilation.GetTypeByMetadataName("Hand.Models.IEntityId");
            if (entityId is null)
                return namedTypeSymbol;
            if (interfaces.Contains(entityId))
                return compilation.GetSpecialType(SpecialType.System_Int64);
            var entityProperty = compilation.GetTypeByMetadataName("Hand.Models.IEntityProperty`1");
            if (entityProperty is null)
                return namedTypeSymbol;
            var @interface = SymbolReflection.GetGenericCloseInterfaces(namedTypeSymbol, entityProperty)
                .FirstOrDefault();
            if (@interface is null)
                return namedTypeSymbol;
            if (@interface.TypeArguments.FirstOrDefault() is INamedTypeSymbol original)
                return original;
        }
        return null;
    }
    /// <summary>
    /// 获取属性字典
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IDictionary<string, IPropertySymbol> GetProperties(INamedTypeSymbol type, IRecognizer<string>[] rules)
    {
        IDictionary<string, IPropertySymbol> properties = SymbolReflection.GetProperties(type).ToDictionary(p => p.Name);
        foreach (var rule in rules)
        {
            properties = rule.Recognize(properties);
        }
        return properties;
    }
}
