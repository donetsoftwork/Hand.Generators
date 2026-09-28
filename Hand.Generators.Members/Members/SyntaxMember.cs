using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Hand.Members;

/// <summary>
/// 语法成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
/// <param name="summary"></param>
public abstract class SyntaxMember(string name, CSharpSyntaxNode original, ITypeSymbolInfo symbolInfo, string summary)
    : MemberBase(name, symbolInfo)
{

    #region 配置
    private readonly CSharpSyntaxNode _original = original;
    private readonly string _summary = summary;
    private readonly Lazy<XmlElementSyntax?> _xmlElement = new(original.GetSummary);
    private readonly List<AttributeData> _attributes = [];
    /// <summary>
    /// 原始成员
    /// </summary>
    public CSharpSyntaxNode Original
        => _original;
    /// <inheritdoc />
    public override string Summary
        => _summary;
    /// <inheritdoc />
    public override XmlElementSyntax? XmlElement
        => _xmlElement.Value;
    /// <inheritdoc />
    public override ImmutableArray<AttributeData> GetAttributes()
        => [.. _attributes];
    #endregion
    /// <summary>
    /// 添加特性标记
    /// </summary>
    /// <param name="attributes"></param>
    public void Add(params IEnumerable<AttributeData> attributes)
        => _attributes.AddRange(attributes);
}
