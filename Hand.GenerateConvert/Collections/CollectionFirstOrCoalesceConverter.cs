using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 获取集合第一个或默认值
/// 调用FirstOrDefault
/// </summary>
/// <param name="method"></param>
/// <param name="isNullable"></param>
/// <param name="defaultValue"></param>
public class CollectionFirstOrCoalesceConverter(SimpleNameSyntax method, bool isNullable, ExpressionSyntax defaultValue)
     : ExtensionMethodConverter(EnumerableConverter.UsingLinq, method, isNullable)
{
    #region 配置
    private readonly ExpressionSyntax _defaultValue = defaultValue;
    private static readonly SimpleNameSyntax _methodName = SyntaxFactory.IdentifierName("FirstOrDefault");
    #endregion
    /// <summary>
    /// 转化Enumerable为数组
    /// </summary>
    public CollectionFirstOrCoalesceConverter(bool isNullable, ExpressionSyntax defaultValue)
        : this(_methodName, isNullable, defaultValue)
    {
    }
    /// <inheritdoc />
    public override ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        var convertExpression = base.Convert(generator, source);
        // 如果_defaultValue为null,NullCoalesce没有意义
        if (_isNullable && _defaultValue.IsKind(SyntaxKind.DefaultExpression))
            return convertExpression;
        else
            return convertExpression.NullCoalesce(_defaultValue);
    }
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="isNullable"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static CollectionFirstOrCoalesceConverter Generic(TypeSyntax argumentType, bool isNullable, ExpressionSyntax defaultValue)
        => new(SyntaxGenerator.Generic(_methodName.Identifier, argumentType), isNullable, defaultValue);
}
