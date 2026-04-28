using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 处理特性标记扩展方法
/// </summary>
public static partial class GenerateCoreServices
{
    /// <summary>
    /// 转化特性数据为特性语法
    /// </summary>
    /// <param name="data"></param>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public static AttributeSyntax ToSyntax(this AttributeData data, List<string> namespaces)
    {
        var type = data.AttributeClass!;
        namespaces.Add(type.ContainingNamespace.ToDisplayString());
        var name = type.Name;
        if (name.EndsWith("Attribute"))
            name = name.Substring(0, name.Length - "Attribute".Length);
        var arguments = new List<AttributeArgumentSyntax>();
        foreach (var item in data.ConstructorArguments)
            arguments.Add(ToAttributeArgument(item));
        foreach (var item in data.NamedArguments)
            arguments.Add(ToAttributeArgument(item.Value, item.Key));

        if (arguments.Count == 0)
            return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(name));
        return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(name))
            .AddArgumentListArguments(arguments.ToArray());
    }
    /// <summary>
    /// 转化特性数据为特性语法数组
    /// </summary>
    /// <param name="datas"></param>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public static AttributeListSyntax[] ToSyntax(this ImmutableArray<AttributeData> datas, List<string> namespaces)
    {
        var count = datas.Length;
        var result = new AttributeListSyntax[count];
        for ( var i = 0; i < count; i++)
            result[i] = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(ToSyntax(datas[i], namespaces)));
        return result;
    }
    /// <summary>
    /// 转化特性参数列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="namedArguments"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeArgumentSyntax> ConvertToArguments(ImmutableArray<TypedConstant> arguments, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
    {
        foreach (var item in arguments)
            yield return ToAttributeArgument(item);
        foreach (var item in namedArguments)
            yield return ToAttributeArgument(item.Value, item.Key);
    }
    /// <summary>
    /// 转化常量为特性参数
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeArgumentSyntax ToAttributeArgument(this TypedConstant argument)
        => SyntaxFactory.AttributeArgument(ToExpression(argument));
    /// <summary>
    /// 转化常量为特性参数
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeArgumentSyntax ToAttributeArgument(this TypedConstant argument, string name)
        => SyntaxFactory.AttributeArgument(ToExpression(argument))
            .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(name)));
}
