using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 转化一个List到另一个List
/// </summary>
/// <param name="methodName"></param>
/// <param name="itemConverter"></param>
public class ListConverter(SimpleNameSyntax methodName, IConverter itemConverter)
      : InstanceMethodConverter(methodName)
{
    /// <summary>
    /// 转化一个List到另一个List
    /// </summary>
    /// <param name="itemConverter"></param>
    public ListConverter(IConverter itemConverter)
        : this(_methodName, itemConverter)
    {
    }
    #region 配置
    private static readonly SimpleNameSyntax _methodName = SyntaxFactory.IdentifierName("ConvertAll");
    private readonly IConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public IConverter ItemConverter
        => _itemConverter;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [EnumerableConverter.GetLambda(generator, _itemConverter)];
}
