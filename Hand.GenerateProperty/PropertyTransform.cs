using Hand.Generators;
using Hand.Reflection;
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
    public PropertySource? Transform(AttributeContext context, CancellationToken cancellation = default)
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
        var originalSymbol = SymbolReflection.GetOriginalSymbol(compilation, symbol);
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        if (originalSymbol == null)
            return null;
        var attributeType = compilation.GetTypeByMetadataName(PropertyGenerator.Attribute);
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
    /// 单例
    /// </summary>
    public static readonly PropertyTransform Instance = new();
}
