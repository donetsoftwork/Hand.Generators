using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enums;

/// <summary>
/// 字符串转化为枚举
/// </summary>
/// <param name="enumType"></param>
/// <param name="ignoreCase"></param>
public sealed class EnumParseConverter(TypeSyntax enumType, ArgumentSyntax ignoreCase)
    : StaticMethodConverter(SyntaxGenerator.Generic(_parse, enumType))
{
    /// <summary>
    /// 字符串转化为枚举
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="ignoreCase"></param>
    public EnumParseConverter(TypeSyntax enumType, LiteralExpressionSyntax ignoreCase)
        : this(enumType, SyntaxFactory.Argument(ignoreCase))
    {
    }
    /// <summary>
    /// 字符串转化为枚举
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="ignoreCase"></param>
    public EnumParseConverter(TypeSyntax enumType, bool ignoreCase = true)
        : this(enumType, SyntaxGenerator.Literal(ignoreCase))
    {

    }
    #region 配置
    private static readonly SyntaxToken _parse = SyntaxFactory.Identifier("Enum.Parse");
    private readonly TypeSyntax _enumType = enumType;
    private readonly ArgumentSyntax _ignoreCase = ignoreCase;

    /// <summary>
    /// 枚举类型
    /// </summary>
    public TypeSyntax EnumType 
        => _enumType;
    /// <summary>
    /// 忽略大小写
    /// </summary>
    public ArgumentSyntax IgnoreCase 
        => _ignoreCase;
    #endregion
    ///// <summary>
    ///// 字符串转化为枚举
    ///// </summary>
    ///// <param name="source">要转换的字符串表达式</param>
    ///// <param name="enumType">目标枚举类型</param>
    ///// <param name="ignoreCase"></param>
    ///// <returns>转换后的枚举表达式</returns>
    //public static ExpressionSyntax Convert(ExpressionSyntax source, TypeSyntax enumType, LiteralExpressionSyntax ignoreCase)
    //{
    //    return SyntaxGenerator.Generic(_parse, enumType)
    //        .Invocation([source, ignoreCase]);
    //}
    /// <inheritdoc />
    public override ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        generator.Using("System");
        return base.Convert(generator, source);
    }
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source), _ignoreCase];
}
