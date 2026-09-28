using Hand.Converters.Methods;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 数组转化器
/// </summary>
/// <param name="type"></param>
/// <param name="method"></param>
/// <param name="itemConverter"></param>
public class ArrayConverter(ISyntaxDisplay<TypeSyntax> type, ISyntaxDisplay<SimpleNameSyntax> method, ISyntaxConverter itemConverter)
     : StaticMethodConverter(type, method)
{
    #region 配置
    private static readonly TypeNameInfo _array = new("Array", "System", false, false);
    private static readonly SyntaxWrapper<SimpleNameSyntax> _convertAll = new(SyntaxFactory.IdentifierName("ConvertAll"));
    private readonly ISyntaxConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public ISyntaxConverter ItemConverter
        => _itemConverter;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source), ItemLinqConverter.GetLambda(generator, _itemConverter)];

    #region Create
    /// <summary>
    /// 构造数组转化器
    /// </summary>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ArrayConverter Create(ISyntaxConverter itemConverter)
        => new(_array, _convertAll, itemConverter);
    /// <summary>
    /// 构造泛型数组转化器
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ArrayConverter Create(ISyntaxDisplay<TypeSyntax> argumentType, ISyntaxConverter itemConverter)
        => new(_array, new GenericDisplay(_convertAll.Original.Identifier, argumentType), itemConverter);
    #endregion
}
