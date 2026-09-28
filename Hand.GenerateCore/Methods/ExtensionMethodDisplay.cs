using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Methods;

/// <summary>
/// 扩展方法名展示
/// </summary>
/// <param name="directive"></param>
/// <param name="original"></param>
public class ExtensionMethodDisplay(UsingDirectiveSyntax directive, ISyntaxDisplay<SimpleNameSyntax> original)
    : ISyntaxDisplay<SimpleNameSyntax>
{
    #region 配置
    private readonly UsingDirectiveSyntax _directive = directive;
    private readonly ISyntaxDisplay<SimpleNameSyntax> _original = original;
    /// <summary>
    /// 引用
    /// </summary>
    public UsingDirectiveSyntax Directive
        => _directive;
    /// <summary>
    /// 原始方法名
    /// </summary>
    public ISyntaxDisplay<SimpleNameSyntax> Original
        => _original;
    #endregion

    /// <inheritdoc />
    public SimpleNameSyntax Display(SyntaxGenerator generator)
    {
        generator.Using(_directive);
        return _original.Display(generator);
    }
    #region Create
    /// <summary>
    /// 构造扩展方法名
    /// </summary>
    /// <param name="directive"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static ExtensionMethodDisplay Create(UsingDirectiveSyntax directive, SimpleNameSyntax methodName)
        => new(directive, new SyntaxWrapper<SimpleNameSyntax>(methodName));
    /// <summary>
    /// 构造扩展方法名
    /// </summary>
    /// <param name="directive"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static ExtensionMethodDisplay Create(UsingDirectiveSyntax directive, string methodName)
        => new(directive, new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(methodName)));
    /// <summary>
    /// 构造扩展方法名
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static ExtensionMethodDisplay Create(string @namespace, string methodName)
        => new(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace)), new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(methodName)));
    #endregion
}
