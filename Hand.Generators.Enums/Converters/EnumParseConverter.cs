using Hand.Converters.Methods;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enums;

/// <summary>
/// 枚举解析器
/// </summary>
/// <param name="type"></param>
/// <param name="method"></param>
/// <param name="ignoreCase"></param>
public sealed class EnumParseConverter(ISyntaxDisplay<TypeSyntax> type, ISyntaxDisplay<SimpleNameSyntax> method, ArgumentSyntax ignoreCase)
    : StaticMethodConverter(type, method)
{
    #region 配置
    private static readonly TypeNameInfo _enum = new("Enum", "System", false, false);
    private static readonly SyntaxToken _parse = SyntaxFactory.Identifier("Parse");
    //private readonly EnumTypeInfo _enumInfo = enumInfo;
    private readonly ArgumentSyntax _ignoreCase = ignoreCase;

    ///// <summary>
    ///// 枚举类型
    ///// </summary>
    //public EnumTypeInfo EnumInfo
    //    => _enumInfo;
    /// <summary>
    /// 忽略大小写
    /// </summary>
    public ArgumentSyntax IgnoreCase 
        => _ignoreCase;
    #endregion

    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source), _ignoreCase];

    /// <summary>
    /// 构造枚举解析器
    /// </summary>
    /// <param name="enumInfo"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static EnumParseConverter Create(EnumTypeInfo enumInfo, bool ignoreCase = true)
        => new(_enum, new GenericDisplay(_parse, enumInfo), SyntaxFactory.Argument(SyntaxGenerator.Literal(ignoreCase)));

    /// <summary>
    /// 字符串转化为枚举
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <param name="enumInfo"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source, EnumTypeInfo enumInfo, bool ignoreCase = true)
    {
        var type = _enum.Display(generator);
        var enumType = enumInfo.Display(generator);
        var genericMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            type,
            SyntaxGenerator.Generic(_parse, enumType));
        return genericMethod.Invocation([SyntaxFactory.Argument(source), SyntaxFactory.Argument(SyntaxGenerator.Literal(ignoreCase))]);
    }
}
