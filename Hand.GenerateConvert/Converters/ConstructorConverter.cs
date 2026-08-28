using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 用构造函数转化类型
/// </summary>
public class ConstructorConverter(ITypeSymbolInfo targetType)
    : IConverter
{
    #region 配置
    private readonly ITypeSymbolInfo _targetType = targetType;

    /// <summary>
    /// 目标类型
    /// </summary>
    public ITypeSymbolInfo TargetType 
        => _targetType;
    #endregion

    ///// <summary>
    ///// 用构造函数转化类型
    ///// </summary>
    ///// <param name="source">源表达式</param>
    ///// <param name="targetType">目标类型</param>
    ///// <returns>转换后的表达式</returns>
    //public static ExpressionSyntax Convert(ExpressionSyntax source, TypeSyntax targetType)
    //    => targetType.New([source]);
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => generator.Display(_targetType).New(CreateArguments(source));
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual SeparatedSyntaxList<ArgumentSyntax> CreateArguments(ExpressionSyntax source)
        => [SyntaxFactory.Argument(source)];
    ///// <inheritdoc />
    //public IConverter Nullable(ExpressionSyntax? defaultExpression)
    //    => this.CheckNull(defaultExpression);
}
