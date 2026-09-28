using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 转化为目标类型
/// </summary>
/// <param name="targetType"></param>
public class CastConverter(ITypeSymbolInfo targetType)
     : ISyntaxConverter
{
    #region 配置
    private readonly ITypeSymbolInfo _targetType = targetType;

    /// <summary>
    /// 目标类型
    /// </summary>
    public ITypeSymbolInfo TargetType
        => _targetType;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => SyntaxFactory.CastExpression(_targetType.Display(generator), source);
    /// <summary>
    /// 转化为目标类型
    /// </summary>
    /// <param name="source">要转换的表达式</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>转换后的表达式</returns>
    public static ExpressionSyntax Convert(ExpressionSyntax source, TypeSyntax targetType)
         => SyntaxFactory.CastExpression(targetType, source);
}
