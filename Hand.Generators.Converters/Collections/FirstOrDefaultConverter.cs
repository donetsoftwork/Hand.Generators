using Hand.Converters.Methods;
using Hand.Methods;
using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 调用FirstOrDefault
/// </summary>
/// <param name="method"></param>
public class FirstOrDefaultConverter(ISyntaxDisplay<SimpleNameSyntax> method)
     : MethodConverter(method)
{
    #region 配置
    /// <summary>
    /// 方法名
    /// </summary>
    protected static readonly SyntaxWrapper<SimpleNameSyntax> _methodName = new(SyntaxFactory.IdentifierName("FirstOrDefault"));
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [];
    /// <summary>
    /// 构造FirstOrDefault转化器
    /// </summary>
    /// <returns></returns>
    public static FirstOrDefaultConverter Create()
        => new(new ExtensionMethodDisplay(LinqConverter.UsingLinq, _methodName));
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <returns></returns>
    public static FirstOrDefaultConverter Create(ISyntaxDisplay<TypeSyntax> argumentType)
        => new(new GenericDisplay(_methodName.Original.Identifier, argumentType));
}
