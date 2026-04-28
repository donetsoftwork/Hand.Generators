using Hand.Filters;
using Hand.Generators;
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
            .SelectMany(GetSyntaxTree)
            .SelectMany((syntaxTree, cancellationToken) => GetAttribute(syntaxTree, attributeName, filter, transform, cancellationToken))
            .WithTrackingName("Provider_ByAttribute");
    }
    /// <summary>
    /// 遍历SyntaxTree
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IEnumerable<(SemanticModel SemanticModel, SyntaxTree SyntaxTree)> GetSyntaxTree(Compilation compilation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var syntaxTree in compilation.SyntaxTrees)
            yield return (compilation.GetSemanticModel(syntaxTree), syntaxTree);
    }
    /// <summary>
    /// 遍历AttributeSyntax
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="attributeName"></param>
    /// <param name="filter"></param>
    /// <param name="transform"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> GetAttribute<TSource>((SemanticModel SemanticModel, SyntaxTree SyntaxTree) syntaxTree, string attributeName, ISyntaxFilter filter, IGeneratorTransform<TSource> transform, CancellationToken cancellationToken = default)
    {
        var semanticModel = syntaxTree.SemanticModel;
        var type0 = semanticModel.Compilation.GetTypeByMetadataName(attributeName);
        if (type0 is null)
            yield break;
        foreach (var attribute in syntaxTree.SyntaxTree.GetRoot(cancellationToken).DescendantNodes().OfType<AttributeSyntax>())
        {
            if(GetAttributeSymbol(semanticModel, attribute, cancellationToken) is not IMethodSymbol attributeSymbol)
                continue;
            var attributeType = attributeSymbol.ContainingType;
            if (!SymbolTypeDescriptor.CheckEquals(type0, attributeType))
                continue;
            var targetNode = attribute.Parent?.Parent;
            if (targetNode is null || !filter.Match(targetNode, cancellationToken))
                continue;
            var targetSymbol = semanticModel.GetDeclaredSymbol(targetNode, cancellationToken);
            if(targetSymbol is null)
                continue;
            var attributes = MatchAttributes(targetNode, targetSymbol, attributeType);
            var context = new AttributeContext(attribute, targetNode, targetSymbol, semanticModel, attributes);
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


    //private static ImmutableArray<SyntaxTree> GetSourceGeneratorInfo(Compilation compilation, CancellationToken cancellationToken)
    //{
    //    return [.. compilation.SyntaxTrees];
    //}
    //private static ImmutableArray<SyntaxNode> GetMatchingNodes(
    //ISyntaxHelper syntaxHelper,
    //GlobalAliases globalAliases,
    //SyntaxTree syntaxTree,
    //string name,
    //Func<SyntaxNode, CancellationToken, bool> predicate,
    //CancellationToken cancellationToken)
    //{
    //    var compilationUnit = syntaxTree.GetRoot(cancellationToken);
    //    Debug.Assert(compilationUnit is ICompilationUnitSyntax);

    //    var isCaseSensitive = syntaxHelper.IsCaseSensitive;
    //    var comparison = isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

    //    // As we walk down the compilation unit and nested namespaces, we may encounter additional using aliases local
    //    // to this file. Keep track of them so we can determine if they would allow an attribute in code to bind to the
    //    // attribute being searched for.
    //    var localAliases = Aliases.GetInstance();
    //    var nameHasAttributeSuffix = name.HasAttributeSuffix(isCaseSensitive);

    //    // Used to ensure that as we recurse through alias names to see if they could bind to attributeName that we
    //    // don't get into cycles.
    //    var seenNames = s_stringStackPool.Allocate();
    //    var results = ArrayBuilder<SyntaxNode>.GetInstance();
    //    var attributeTargets = ArrayBuilder<SyntaxNode>.GetInstance();

    //    try
    //    {
    //        processCompilationUnit(compilationUnit);
    //    }
    //    finally
    //    {
    //        localAliases.Free();
    //        seenNames.Clear();
    //        s_stringStackPool.Free(seenNames);
    //        attributeTargets.Free();
    //    }

    //    results.RemoveDuplicates();
    //    return results.ToImmutableAndFree();

    //    void processCompilationUnit(SyntaxNode compilationUnit)
    //    {
    //        cancellationToken.ThrowIfCancellationRequested();

    //        if (compilationUnit is ICompilationUnitSyntax)
    //            syntaxHelper.AddAliases(compilationUnit.Green, localAliases, global: false);

    //        processCompilationOrNamespaceMembers(compilationUnit);
    //    }

    //    void processCompilationOrNamespaceMembers(SyntaxNode node)
    //    {
    //        cancellationToken.ThrowIfCancellationRequested();

    //        foreach (var child in node.ChildNodesAndTokens())
    //        {
    //            if (child.AsNode(out var childNode))
    //            {
    //                if (syntaxHelper.IsAnyNamespaceBlock(childNode))
    //                    processNamespaceBlock(childNode);
    //                else
    //                    processMember(childNode);
    //            }
    //        }
    //    }

    //    void processNamespaceBlock(SyntaxNode namespaceBlock)
    //    {
    //        cancellationToken.ThrowIfCancellationRequested();

    //        var localAliasCount = localAliases.Count;
    //        syntaxHelper.AddAliases(namespaceBlock.Green, localAliases, global: false);

    //        processCompilationOrNamespaceMembers(namespaceBlock);

    //        // after recursing into this namespace, dump any local aliases we added from this namespace decl itself.
    //        localAliases.Count = localAliasCount;
    //    }

    //    void processMember(SyntaxNode member)
    //    {
    //        cancellationToken.ThrowIfCancellationRequested();

    //        // Don't bother descending into nodes that don't contain attributes.
    //        if (!member.ContainsAttributes)
    //            return;

    //        // nodes can be arbitrarily deep.  Use an explicit stack over recursion to prevent a stack-overflow.
    //        var nodeStack = s_nodeStackPool.Allocate();
    //        nodeStack.Push(member);

    //        try
    //        {
    //            while (nodeStack.Count > 0)
    //            {
    //                var node = nodeStack.Pop();

    //                // Don't bother descending into nodes that don't contain attributes.
    //                if (!node.ContainsAttributes)
    //                    continue;

    //                if (syntaxHelper.IsAttributeList(node))
    //                {
    //                    foreach (var attribute in syntaxHelper.GetAttributesOfAttributeList(node))
    //                    {
    //                        // Have to lookup both with the name in the attribute, as well as adding the 'Attribute' suffix.
    //                        // e.g. if there is [X] then we have to lookup with X and with XAttribute.
    //                        var simpleAttributeName = syntaxHelper.GetUnqualifiedIdentifierOfName(syntaxHelper.GetNameOfAttribute(attribute));
    //                        if (matchesAttributeName(simpleAttributeName, withAttributeSuffix: false) ||
    //                            matchesAttributeName(simpleAttributeName, withAttributeSuffix: true))
    //                        {
    //                            attributeTargets.Clear();
    //                            syntaxHelper.AddAttributeTargets(node, attributeTargets);

    //                            foreach (var target in attributeTargets)
    //                            {
    //                                if (predicate(target, cancellationToken))
    //                                    results.Add(target);
    //                            }

    //                            break;
    //                        }
    //                    }

    //                    // attributes can't have attributes inside of them.  so no need to recurse when we're done.
    //                }
    //                else
    //                {
    //                    // For any other node, just keep recursing deeper to see if we can find an attribute. Note: we cannot
    //                    // terminate the search anywhere as attributes may be found on things like local functions, and that
    //                    // means having to dive deep into statements and expressions.
    //                    foreach (var child in node.ChildNodesAndTokens().Reverse())
    //                    {
    //                        if (child.AsNode(out var childNode))
    //                            nodeStack.Push(childNode);
    //                    }
    //                }

    //            }
    //        }
    //        finally
    //        {
    //            nodeStack.Clear();
    //            s_nodeStackPool.Free(nodeStack);
    //        }
    //    }

    //    // Checks if `name` is equal to `matchAgainst`.  if `withAttributeSuffix` is true, then
    //    // will check if `name` + "Attribute" is equal to `matchAgainst`
    //    bool matchesName(string name, string matchAgainst, bool withAttributeSuffix)
    //    {
    //        if (withAttributeSuffix)
    //        {
    //            return name.Length + "Attribute".Length == matchAgainst.Length &&
    //                matchAgainst.HasAttributeSuffix(isCaseSensitive) &&
    //                matchAgainst.StartsWith(name, comparison);
    //        }
    //        else
    //        {
    //            return name.Equals(matchAgainst, comparison);
    //        }
    //    }

    //    bool matchesAttributeName(string currentAttributeName, bool withAttributeSuffix)
    //    {
    //        // If the names match, we're done.
    //        if (withAttributeSuffix)
    //        {
    //            if (nameHasAttributeSuffix &&
    //                matchesName(currentAttributeName, name, withAttributeSuffix))
    //            {
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            if (matchesName(currentAttributeName, name, withAttributeSuffix: false))
    //                return true;
    //        }

    //        // Otherwise, keep searching through aliases.  Check that this is the first time seeing this name so we
    //        // don't infinite recurse in error code where aliases reference each other.
    //        //
    //        // note: as we recurse up the aliases, we do not want to add the attribute suffix anymore.  aliases must
    //        // reference the actual real name of the symbol they are aliasing.
    //        if (seenNames.Contains(currentAttributeName))
    //            return false;

    //        seenNames.Push(currentAttributeName);
    //        try
    //        {
    //            foreach (var (aliasName, symbolName) in localAliases)
    //            {
    //                // see if user wrote `[SomeAlias]`.  If so, if we find a `using SomeAlias = ...` recurse using the
    //                // ... name portion to see if it might bind to the attr name the caller is searching for.
    //                if (matchesName(currentAttributeName, aliasName, withAttributeSuffix) &&
    //                    matchesAttributeName(symbolName, withAttributeSuffix: false))
    //                {
    //                    return true;
    //                }
    //            }

    //            foreach (var (aliasName, symbolName) in globalAliases.AliasAndSymbolNames)
    //            {
    //                if (matchesName(currentAttributeName, aliasName, withAttributeSuffix) &&
    //                    matchesAttributeName(symbolName, withAttributeSuffix: false))
    //                {
    //                    return true;
    //                }
    //            }

    //            return false;
    //        }
    //        finally
    //        {
    //            seenNames.Pop();
    //        }
    //    }
    //}
}
