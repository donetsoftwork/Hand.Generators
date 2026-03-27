using Hand.Generators;
using Hand.Symbols;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;

namespace Hand.GenerateProperty;

/// <summary>
/// 属性转化器
/// </summary>
public class PropertyTransform : IGeneratorTransform<PropertySource>
{
    /// <inheritdoc />
    public PropertySource? Transform(AttributeContext context, CancellationToken cancellation)
    {
//#if DEBUG
//        System.Diagnostics.Debugger.Launch();
//#endif
        if (cancellation.IsCancellationRequested)
            return null;
        if (context.TargetNode is not TypeDeclarationSyntax type)
            return null;
        if(context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;
        var compilation = context.SemanticModel.Compilation;
        var originalSymbol = GetOriginalSymbol(compilation, symbol);
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        if (originalSymbol == null)
            return null;
        var attributeType = compilation.GetTypeByMetadataName("Hand.Entities.GeneratePropertyAttribute");
        if (attributeType is null)
            return null;
        var attribute = SymbolAttributeHelper.GetAttributesByType(context.Attributes, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return null;
        // 获取GenerateProperty的Rules属性
        var ruleText = SymbolAttributeHelper.GetArgumentValue<string>(attribute, 0);
        var rule = new PropertyRule(ruleText);
        return new PropertySource(type, compilation, symbol, originalSymbol, rule);
    }
    /// <summary>
    /// 获取原始类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetOriginalSymbol(Compilation compilation, INamedTypeSymbol symbol)
    {
        var interfaces = symbol.AllInterfaces;
        var entityId = compilation.GetTypeByMetadataName("Hand.Models.IEntityId");
        if (entityId is null)
            return null;
        if (interfaces.Contains(entityId))
            return compilation.GetSpecialType(SpecialType.System_Int64);
        var entityProperty = compilation.GetTypeByMetadataName("Hand.Models.IEntityProperty`1");
        if (entityProperty is null)
            return null;
        var @interface = SymbolReflection.GetGenericCloseInterfaces(symbol, entityProperty)
            .FirstOrDefault();
        if (@interface is null)
            return null;
        return @interface.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
    }

    /// <summary>
    /// 单例
    /// </summary>
    public static readonly PropertyTransform Instance = new();
}
