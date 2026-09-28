using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 处理特性标记扩展方法
/// </summary>
public static partial class ReflectionServices
{
    /// <summary>
    /// 生成特性标记
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="member"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static TMember GenerateAttribute<TMember>(this SyntaxGenerator generator, TMember member, AttributeData[] attributes)
        where TMember : CSharpSyntaxNode
    {
        if (attributes.Length == 0)
            return member;
        List<string> namespaces = [];
        if (member is MemberDeclarationSyntax memberDeclaration)
            member = (TMember)(CSharpSyntaxNode)memberDeclaration.WithAttributeLists(SyntaxFactory.List(attributes.ToSyntax(namespaces)));
        else if (member is ParameterSyntax parameterSyntax)
            member = (TMember)(CSharpSyntaxNode)parameterSyntax.WithAttributeLists(SyntaxFactory.List(attributes.ToSyntax(namespaces)));

        foreach (var item in namespaces.Distinct())
            generator.Using(item);
        return member;
    }
    /// <summary>
    /// 转化特性数据为特性语法
    /// </summary>
    /// <param name="data"></param>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public static AttributeSyntax ToSyntax(this AttributeData data, List<string> namespaces)
    {
        var type = data.AttributeClass!;
        var @namespace = type.ContainingNamespace.ToDisplayString();
        if (!string.IsNullOrWhiteSpace(@namespace))
            namespaces.Add(@namespace);
        var name = type.Name;
        if (name.EndsWith("Attribute"))
            name = name.Substring(0, name.Length - "Attribute".Length);
        var arguments = ConvertToArguments(data.ConstructorArguments, data.NamedArguments)
            .ToArray();
        //foreach (var item in data.ConstructorArguments)
        //    arguments.Add(ToExpression(item).ToAttributeArgument());
        //foreach (var item in data.NamedArguments)
        //    arguments.Add(ToExpression(item.Value).ToAttributeArgument(item.Key));
        if (arguments.Length == 0)
            return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(name));
        return SyntaxFactory.IdentifierName(name)
            .Attribute([..arguments]);
    }
    /// <summary>
    /// 转化特性数据为特性语法数组
    /// </summary>
    /// <param name="datas"></param>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeListSyntax[] ToSyntax(this AttributeData[] datas, List<string> namespaces)
        => Array.ConvertAll(datas, data => ToSyntax(data, namespaces).ToSingletonList());
    /// <summary>
    /// 转化特性参数列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="namedArguments"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeArgumentSyntax> ConvertToArguments(ImmutableArray<TypedConstant> arguments, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
    {
        foreach (var item in arguments)
            yield return ToExpression(item)
                .ToAttributeArgument();
        foreach (var item in namedArguments)
            yield return ToExpression(item.Value)
                .ToAttributeArgument(item.Key);
    }
}
