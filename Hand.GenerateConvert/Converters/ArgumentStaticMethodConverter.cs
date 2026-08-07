using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Converters;

/// <summary>
/// 使用静态方法转化
/// </summary>
/// <param name="method"></param>
/// <param name="otherArguments"></param>
public class ArgumentStaticMethodConverter(ExpressionSyntax method, IEnumerable<ArgumentSyntax> otherArguments)
    : StaticMethodConverter(method)
{
    /// <summary>
    /// 使用静态方法转化
    /// </summary>
    /// <param name="method"></param>
    /// <param name="otherArguments"></param>
    public ArgumentStaticMethodConverter(ExpressionSyntax method, params IEnumerable<ExpressionSyntax> otherArguments)
        : this(method, otherArguments.Select(SyntaxFactory.Argument))
    {
    }
    /// <summary>
    /// 使用静态方法转化
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="otherArguments"></param>
    public ArgumentStaticMethodConverter(string methodName, params IEnumerable<ExpressionSyntax> otherArguments)
        : this(SyntaxFactory.IdentifierName(methodName), otherArguments.Select(SyntaxFactory.Argument))
    {
    }
    #region 配置
    private readonly IEnumerable<ArgumentSyntax> _otherArguments = otherArguments;
    /// <summary>
    /// 其他参数
    /// </summary>
    public IEnumerable<ArgumentSyntax> OtherArguments
        => _otherArguments;
    #endregion
    /// <inheritdoc />
    protected override IEnumerable<ArgumentSyntax> CreateArguments(ExpressionSyntax source)
        => [SyntaxFactory.Argument(source), .. _otherArguments];
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
