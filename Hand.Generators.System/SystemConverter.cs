using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.System;

/// <summary>
/// 系统类型转化器
/// </summary>
/// <param name="methodName">要使用的方法名</param>
public class SystemConverter(SimpleNameSyntax methodName)
    : ISyntaxConverter
{
    #region 配置
    private static readonly TypeSyntax _convertTypeName = SyntaxFactory.IdentifierName("Convert");
    private readonly SimpleNameSyntax _methodName = methodName;

    /// <summary>
    /// 方法名
    /// </summary>
    public SimpleNameSyntax MethodName 
        => _methodName;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        generator.UsingSystem();
        return _convertTypeName.Access(_methodName)
            .Invocation([source]);
    }
}
