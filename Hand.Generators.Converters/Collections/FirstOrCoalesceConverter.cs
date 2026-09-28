using Hand.Methods;
using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 获取集合第一个或默认值
/// 调用FirstOrDefault
/// </summary>
/// <param name="method"></param>
/// <param name="defaultValue"></param>
public class FirstOrCoalesceConverter(ISyntaxDisplay<SimpleNameSyntax> method, ExpressionSyntax defaultValue)
     : FirstOrDefaultConverter(method)
{
    #region 配置
    private readonly ExpressionSyntax _defaultValue = defaultValue;
    #endregion
    /// <summary>
    /// 转化Enumerable为数组
    /// </summary>
    public FirstOrCoalesceConverter(ExpressionSyntax defaultValue)
        : this(_methodName, defaultValue)
    {
    }
    /// <inheritdoc />
    public override ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => base.Convert(generator, source)
        .NullCoalesce(_defaultValue);


    /// <summary>
    /// 构造FirstOrDefault转化器
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static FirstOrCoalesceConverter Create(ExpressionSyntax defaultValue)
        => new(new ExtensionMethodDisplay(LinqConverter.UsingLinq, _methodName), defaultValue);
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static FirstOrCoalesceConverter Create(ISyntaxDisplay<TypeSyntax> argumentType, ExpressionSyntax defaultValue)
        => new(new GenericDisplay(_methodName.Original.Identifier, argumentType), defaultValue);
}
