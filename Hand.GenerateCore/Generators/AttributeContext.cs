using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;

namespace Hand.Generators;

/// <summary>
/// 特性上下文
/// </summary>
/// <param name="attribute"></param>
/// <param name="targetNode"></param>
/// <param name="targetSymbol"></param>
/// <param name="semanticmodel"></param>
/// <param name="attributes"></param>
public readonly struct AttributeContext(AttributeSyntax attribute, SyntaxNode targetNode, ISymbol targetSymbol, SemanticModel semanticmodel, ImmutableArray<AttributeData> attributes)
{
    /// <summary>
    /// 特性
    /// </summary>
    public AttributeSyntax Attribute { get; } = attribute;
    /// <summary>
    /// 所属节点
    /// </summary>
    public SyntaxNode TargetNode { get; } = targetNode;
    /// <summary>
    /// 所属节点符号
    /// </summary>
    public ISymbol TargetSymbol { get; } = targetSymbol;
    /// <summary>
    /// 语义模型
    /// </summary>
    public SemanticModel SemanticModel { get; } = semanticmodel;
    /// <summary>
    /// 特性标记
    /// </summary>
    public ImmutableArray<AttributeData> Attributes { get; } = attributes;
}
