using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Constructors;

/// <summary>
/// 用构造函数转化类型
/// </summary>
public class ConstructorConverter(ISyntaxDisplay<TypeSyntax> targetType)
    : ISyntaxConverter
{
    #region 配置
    private readonly ISyntaxDisplay<TypeSyntax> _targetType = targetType;

    /// <summary>
    /// 目标类型
    /// </summary>
    public ISyntaxDisplay<TypeSyntax> TargetType 
        => _targetType;
    #endregion

    /// <inheritdoc />
    public virtual ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => _targetType.Display(generator).New(CreateArguments(generator, source), CreateInitializer(generator));
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source)];
    /// <summary>
    /// 构造初始化器
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    protected virtual InitializerExpressionSyntax? CreateInitializer(SyntaxGenerator generator)
        => null;
}
