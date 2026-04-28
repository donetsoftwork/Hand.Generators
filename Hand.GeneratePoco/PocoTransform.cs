using Hand.Generators;
using Hand.Maping;
using Hand.Rule;
using Hand.Symbols;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hand.GeneratePoco;

/// <summary>
/// 转化
/// </summary>
public class PocoTransform : IGeneratorTransform<PocoSource>
{
    /// <inheritdoc />
    public PocoSource? Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
            return null;
        if (context.TargetNode is not TypeDeclarationSyntax type)
            return null;
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;
        var compilation = context.SemanticModel.Compilation;
        var attributeType = compilation.GetTypeByMetadataName(PocoGenerator.Attribute);
        if (attributeType is null)
            return null;
        var attribute = SymbolAttributeHelper.GetAttributesByType(context.Attributes, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return null;
        //var from = SymbolAttributeHelper.GetArgumentValue<Type>(attribute, 0);
        //if (from is null)
        //    return null;
        //var fromSymbol = compilation.GetTypeByMetadataName(from.FullName);
        //if (fromSymbol is null) 
        //    return null;
        var fromSymbol = SymbolAttributeHelper.GetArgumentValue<INamedTypeSymbol>(attribute, 0);
        if (fromSymbol is null)
            return null;
        var ruleTexts = SymbolAttributeHelper.GetArgumentValues<string>(attribute, "Rules");
        var rules = ParsePropertyRules([.. ruleTexts]);
        var nullableText = SymbolAttributeHelper.GetArgumentValue<string>(attribute, "NullableRule");
        var nullableRule = CheckNullableRule(nullableText);
        var init = SymbolAttributeHelper.GetArgumentValue<bool>(attribute, "Init");

        return new PocoSource(type, compilation, symbol, fromSymbol, rules, nullableRule, init);
    }
    /// <summary>
    /// 解析属性投影规则
    /// </summary>
    /// <param name="texts"></param>
    /// <returns></returns>
    public static IRecognizer<string>[] ParsePropertyRules(string[] texts)
    {
        if(texts is null)
            return [];
        var count = texts.Length;
        if (count == 0) 
            return [];
        var list = new List<IRecognizer<string>>(count);
        foreach (var text in texts)
        {
            if(string.IsNullOrEmpty(text)) 
                continue;
            list.Add(MemberRecognizeParser.Default.Parse(text));
        }
        return [.. list];
    }
    /// <summary>
    /// 可空规则解析
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static IValidation<string>? CheckNullableRule(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return null;
        return MemberRuleParser.Default.Parse(text);
    }

    /// <summary>
    /// 单例
    /// </summary>
    public static readonly PocoTransform Instance = new();
}
