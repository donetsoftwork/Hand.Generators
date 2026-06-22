using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Converters;

/// <summary>
/// 使用静态方法转化
/// </summary>
/// <param name="method"></param>
/// <param name="otherArguments"></param>
public class ArgumentStaticMethodConverter(ExpressionSyntax method, params ExpressionSyntax[] otherArguments)
    : StaticMethodConverter(method)
{
    /// <summary>
    /// 使用静态方法转化
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="otherArguments"></param>
    public ArgumentStaticMethodConverter(string methodName, params ExpressionSyntax[] otherArguments)
        : this(SyntaxFactory.IdentifierName(methodName), otherArguments)
    {
    }
    #region 配置
    private readonly ExpressionSyntax[] _otherArguments = otherArguments;
    /// <summary>
    /// 其他参数
    /// </summary>
    public ExpressionSyntax[] OtherArguments
        => _otherArguments;
    #endregion
    /// <inheritdoc />
    protected override IEnumerable<ExpressionSyntax> CreateArguments(ExpressionSyntax source)
        => [source, .. _otherArguments];
    ///// <summary>
    ///// 使用静态方法转化
    ///// </summary>
    ///// <param name="methodName"></param>
    ///// <param name="source"></param>
    ///// <param name="otherArguments"></param>
    ///// <returns></returns>
    //public static ExpressionSyntax Convert(SimpleNameSyntax methodName, ExpressionSyntax source, params ExpressionSyntax[] otherArguments)
    //    => methodName.Invocation([source, .. otherArguments]);
}
