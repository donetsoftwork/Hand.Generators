using Hand.Methods;
using Hand.Scopes;
using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enumerations;

/// <summary>
/// 枚举提供器
/// </summary>
/// <param name="provider"></param>
/// <param name="name"></param>
/// <param name="original"></param>
public class EnumerationProvider(ISyntaxDisplay<ExpressionSyntax> provider, ISyntaxDisplay<SimpleNameSyntax> name, ISyntaxDisplay<SimpleNameSyntax> original)
{
    /// <summary>
    /// 扩展方法Using
    /// </summary>
    private static readonly UsingDirectiveSyntax _using = SyntaxFactory.UsingDirective(NamespaceScope.Hand);
    /// <summary>
    /// Get扩展方法
    /// </summary>
    private static readonly ISyntaxDisplay<SimpleNameSyntax> _getMethod = ExtensionMethodDisplay.Create(_using, SyntaxFactory.IdentifierName("Get"));
    /// <summary>
    /// Parse方法
    /// </summary>
    private static readonly ISyntaxDisplay<SimpleNameSyntax> _parselMethod = new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName("Parse"));


    private readonly ISyntaxDisplay<ExpressionSyntax> _provider = provider;
    private readonly ISyntaxDisplay<SimpleNameSyntax> _name = name;
    private readonly ISyntaxDisplay<SimpleNameSyntax> _original = original;
    //private readonly bool _isNullable = isNullable;
    ///// <summary>
    ///// 是否可能有空
    ///// </summary>
    //public bool IsNullable 
    //    => _isNullable;
    /// <summary>
    /// 获取Get方法
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private ExpressionSyntax CheckMethod(SyntaxGenerator generator, ISyntaxDisplay<SimpleNameSyntax> method)
        => _provider.Display(generator).Access(method.Display(generator));
    /// <summary>
    /// 按枚举名获取
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual ExpressionSyntax GetByName(SyntaxGenerator generator, ExpressionSyntax name)
        => CheckMethod(generator, _name).Invocation([name]);
    /// <summary>
    /// 按原始值获取
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public virtual ExpressionSyntax GetByOriginal(SyntaxGenerator generator, ExpressionSyntax original)
        => CheckMethod(generator, _original).Invocation([original]);
    #region Create
    /// <summary>
    /// 创建枚举提供器
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public static EnumerationProvider Create(ISyntaxDisplay<ExpressionSyntax> provider, IMethodSymbol name, IMethodSymbol original)
        => new(provider, new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(name.Name)), new SyntaxWrapper<SimpleNameSyntax>(SyntaxFactory.IdentifierName(original.Name)));
    /// <summary>
    /// 创建枚举提供器
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static EnumerationProvider CreateDefault(ISyntaxDisplay<ExpressionSyntax> provider)
        => new(provider, _getMethod, _getMethod);
    /// <summary>
    /// 创建位标记枚举提供器
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static EnumerationProvider CreateFlag(ISyntaxDisplay<ExpressionSyntax> provider)
        => new(provider, _parselMethod, _parselMethod);
    #endregion
}
