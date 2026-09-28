using Hand.Methods;
using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Methods;

/// <summary>
/// 方法转化器
/// </summary>
/// <param name="method"></param>
public class MethodConverter(ISyntaxDisplay<SimpleNameSyntax> method)
     : ISyntaxConverter
{
    #region 配置
    private readonly ISyntaxDisplay<SimpleNameSyntax> _method = method;
    /// <summary>
    /// 方法
    /// </summary>
    public ISyntaxDisplay<SimpleNameSyntax> Method 
        => _method;
    #endregion

    /// <inheritdoc />
    public virtual ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        return GetMethod(generator, source)
            .Invocation(CreateArguments(generator, source));
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual ExpressionSyntax GetMethod(SyntaxGenerator generator, ExpressionSyntax source)
        => source.Access(_method.Display(generator));
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [];

    #region Create
    /// <summary>
    /// 构造方法转化器
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodConverter Create(SimpleNameSyntax methodName)
        => new(new SyntaxWrapper<SimpleNameSyntax>(methodName));
    /// <summary>
    /// 构造方法转化器
    /// </summary>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodConverter Create(string methodName)
    => new(new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(methodName)));
    /// <summary>
    /// 构造扩展方法转化器
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodConverter CreateExtensionMethod(UsingDirectiveSyntax @namespace, SimpleNameSyntax methodName)
        => new(new ExtensionMethodDisplay(@namespace, new SyntaxWrapper<SimpleNameSyntax>(methodName)));
    /// <summary>
    /// 构造扩展方法转化器
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodConverter CreateExtensionMethod(string @namespace, string methodName)
        => new(new ExtensionMethodDisplay(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace)), new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(methodName))));
    #endregion
}
