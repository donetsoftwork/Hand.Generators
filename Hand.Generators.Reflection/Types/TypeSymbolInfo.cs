using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 类型信息
/// </summary>
/// <param name="original"></param>
public class TypeSymbolInfo(INamedTypeSymbol original)
    : ISyntaxDisplay<TypeSyntax>
{
    #region 配置
    /// <summary>
    /// 原始类型
    /// </summary>
    protected readonly INamedTypeSymbol _original = original;
    /// <summary>
    /// 原始类型
    /// </summary>
    public INamedTypeSymbol Original
        => _original;
    #endregion
    /// <inheritdoc />
    public virtual TypeSyntax Display(SyntaxGenerator generator)
        => generator.Display(_original);
}
