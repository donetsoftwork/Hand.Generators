using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 获取集合第一个或默认值
/// 调用FirstOrDefault&lt;TSource&gt;(IEnumerable&lt;TSource&gt;, TSource)
/// .NET支持版本 6, 7, 8, 9, 10, 11
/// </summary>
/// <param name="method"></param>
/// <param name="defaultValue"></param>
public class FirstOrDefaultValueConverter(ISyntaxDisplay<SimpleNameSyntax> method, ArgumentSyntax defaultValue)
     : FirstOrDefaultConverter(method)
{
    #region 配置
    /// <summary>
    /// 默认值
    /// </summary>
    protected readonly ArgumentSyntax _defaultValue = defaultValue;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [_defaultValue];

    /// <summary>
    /// 构造默认值转化器
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static FirstOrDefaultValueConverter Create(ExpressionSyntax defaultValue)
        => new(_methodName, SyntaxFactory.Argument(defaultValue));
    /// <summary>
    /// 构造泛型默认值转化器
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static FirstOrDefaultValueConverter Create(ISyntaxDisplay<TypeSyntax> argumentType, ExpressionSyntax defaultValue)
        => new(new GenericDisplay(_methodName.Original.Identifier, argumentType), SyntaxFactory.Argument(defaultValue));
}
