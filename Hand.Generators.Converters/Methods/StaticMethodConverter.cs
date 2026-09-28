using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Methods;

/// <summary>
/// 使用静态方法转化
/// </summary>
/// <param name="type"></param>
/// <param name="method"></param>
public class StaticMethodConverter(ISyntaxDisplay<TypeSyntax> type, ISyntaxDisplay<SimpleNameSyntax> method)
    : ISyntaxConverter
{
    #region 配置
    /// <summary>
    /// 类型名称
    /// </summary>
    private readonly ISyntaxDisplay<TypeSyntax> _type = type;
    /// <summary>
    /// 方法
    /// </summary>
    protected readonly ISyntaxDisplay<SimpleNameSyntax> _method = method;

    /// <summary>
    /// 类型名称
    /// </summary>
    public ISyntaxDisplay<TypeSyntax> Type
        => _type;
    /// <summary>
    /// 方法
    /// </summary>
    public ISyntaxDisplay<SimpleNameSyntax> Method 
        => _method;
    #endregion
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source)];

    /// <inheritdoc />
    public virtual ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        return GetMethod(generator)
            .Invocation(CreateArguments(generator, source));
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <returns></returns>
    protected virtual ExpressionSyntax GetMethod(SyntaxGenerator generator)
    {
        var type = _type.Display(generator);
        var method = _method.Display(generator);
        return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, type, method);
    }

    #region Create
    /// <summary>
    /// 构造方法转化器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static StaticMethodConverter Create(ISyntaxDisplay<TypeSyntax> type, SimpleNameSyntax methodName)
        => new(type, new SyntaxWrapper<SimpleNameSyntax>(methodName));
    /// <summary>
    /// 构造方法转化器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static StaticMethodConverter Create(ISyntaxDisplay<TypeSyntax> type, string methodName)
        => new(type, new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(methodName)));
    /// <summary>
    /// 构造方法转化器
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static StaticMethodConverter Create(INamedTypeSymbol symbol, string methodName)
        => new(TypeNameInfo.GetInfo(symbol), new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(methodName)));
    #endregion
}
