using Hand.Filters;
using Hand.Generators;
using Hand.Reflection;
using Hand.Symbols;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Hand;

/// <summary>
/// 自定义IncrementalValuesProvider
/// </summary>
public class GenerateProvider
{
    /// <summary>
    /// 按Attribute筛选
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="context"></param>
    /// <param name="attributeName"></param>
    /// <param name="filter"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static IncrementalValuesProvider<TSource> CreateByAttribute<TSource>(IncrementalGeneratorInitializationContext context, string attributeName, ISyntaxFilter filter, IGeneratorTransform<TSource> transform)
    {
        return context.CompilationProvider
            .SelectMany((compilation, cancellationToken) => GetModel(compilation, attributeName, cancellationToken))
            .SelectMany((model, cancellationToken) => GetAttribute(model.SemanticModel, model.Checker, model.SyntaxTree, filter, transform, cancellationToken))
            .WithTrackingName("Provider_ByAttribute");
    }
    /// <summary>
    /// 遍历SyntaxTree
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="attributeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IEnumerable<(SemanticModel SemanticModel, Predicate<INamedTypeSymbol> Checker, SyntaxTree SyntaxTree)> GetModel(Compilation compilation, string attributeName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var attributeType = compilation.GetTypeByMetadataName(attributeName);
        if(attributeType is not null)
        {
            var checker = SymbolAttributeHelper.GetAttributeClassChecker(attributeType);
            foreach (var syntaxTree in compilation.SyntaxTrees)
                yield return (compilation.GetSemanticModel(syntaxTree), checker, syntaxTree);
        }
    }
    /// <summary>
    /// 遍历AttributeSyntax
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="checker"></param>
    /// <param name="syntaxTree"></param>
    /// <param name="filter"></param>
    /// <param name="transform"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> GetAttribute<TSource>(SemanticModel semanticModel, Predicate<INamedTypeSymbol> checker, SyntaxTree syntaxTree, ISyntaxFilter filter, IGeneratorTransform<TSource> transform, CancellationToken cancellationToken = default)
    {
        //var semanticModel = syntaxTree.SemanticModel;
        //var type0 = semanticModel.Compilation.GetTypeByMetadataName(attributeName);
        //if (type0 is null)
        //    yield break; 

        foreach (var attribute in syntaxTree.GetRoot(cancellationToken).DescendantNodes().OfType<AttributeSyntax>())
        {
            if(GetAttributeSymbol(semanticModel, attribute, cancellationToken) is not IMethodSymbol attributeSymbol)
                continue;
            var attributeType = attributeSymbol.ContainingType;
            if (!checker(attributeType))
                continue;
            var targetNode = attribute.Parent?.Parent;
            if (targetNode is null || !filter.Match(targetNode, cancellationToken))
                continue;
            var targetSymbol = semanticModel.GetDeclaredSymbol(targetNode, cancellationToken);
            if(targetSymbol is null || targetSymbol.DeclaredAccessibility == Accessibility.Private)
                continue;
            var attributes = MatchAttributes(targetNode, targetSymbol, attributeType);
            var context = new AttributeContext(targetNode, targetSymbol, semanticModel, attributes);
            var source = transform.Transform(context, cancellationToken);
            if (source is null)
                continue;
            yield return source;
        }
       
    }
    /// <summary>
    /// 获取特性标记符号
    /// </summary>
    /// <param name="semanticModel"></param>
    /// <param name="attribute"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static ISymbol? GetAttributeSymbol(SemanticModel semanticModel, AttributeSyntax attribute, CancellationToken cancellationToken = default)
    {
        var info = semanticModel.GetSymbolInfo(attribute, cancellationToken);
        return info.Symbol ?? info.CandidateSymbols.FirstOrDefault();
    }
    /// <summary>
    /// 匹配特性
    /// </summary>
    /// <param name="targetNode"></param>
    /// <param name="targetSymbol"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    public static ImmutableArray<AttributeData> MatchAttributes(SyntaxNode targetNode, ISymbol targetSymbol, INamedTypeSymbol attributeType)
    {
        var targetSyntaxTree = targetNode.SyntaxTree;
        var result = ImmutableArray.CreateBuilder<AttributeData>();
        foreach (var attribute in targetSymbol.GetAttributes())
            Add(attribute);
        return result.ToImmutable();

        void Add(AttributeData attribute)
        {
            var reference = attribute.ApplicationSyntaxReference;
            if (reference is null)
                return;
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null || attributeClass.TypeKind == TypeKind.Error)
                return;
            if(reference.SyntaxTree == targetSyntaxTree && SymbolTypeDescriptor.CheckEquals(attributeClass, attributeType))
                result.Add(attribute);
        }
    }
}
