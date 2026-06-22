using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 系统类型转化器
/// </summary>
/// <param name="methodName">要使用的方法名</param>
public class SystemConverter(SimpleNameSyntax methodName)
    : IConverter
{
    #region 配置
    private static readonly TypeSyntax _convertTypeName = SyntaxFactory.IdentifierName("System.Convert");
    private readonly SimpleNameSyntax _methodName = methodName;

    /// <summary>
    /// 方法名
    /// </summary>
    public SimpleNameSyntax MethodName 
        => _methodName;
    #endregion
    ///// <summary>
    ///// 使用指定方法转化
    ///// </summary>
    ///// <param name="source">要转化的表达式</param>
    ///// <param name="methodName">要使用的方法名</param>
    ///// <returns>转化后的表达式</returns>
    //public static ExpressionSyntax Convert(ExpressionSyntax source, string methodName)
    //{
    //    var exp = _convertTypeName.Access(methodName).Invocation([source]);
    //}
    /// <inheritdoc />
    public ExpressionSyntax Convert(ExpressionSyntax source)
    {
        return _convertTypeName.Access(_methodName)
            .Invocation([source]);
    }
}
