using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 预定义类型信息
/// </summary>
public class PredefinedTypeInfo(SyntaxKind kind, INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    : PrimitiveTypeInfo(original, symbol, isNullable), ITypeSymbolInfo
{
    #region 配置
    private readonly SyntaxKind _kind = kind;
    /// <summary>
    /// 符号种类
    /// </summary>
    public SyntaxKind Kind 
        => _kind;
    #endregion

    /// <inheritdoc />
    public override TypeSyntax Display(SyntaxGenerator generator)
        => SyntaxFactory.PredefinedType(SyntaxFactory.Token(_kind)).Nullable(_isNullable);
    /// <inheritdoc />
    public override TypeOfExpressionSyntax TypeOf(SyntaxGenerator generator)
        => SyntaxFactory.TypeOfExpression(_symbol.IsValueType ? Display(generator) : SyntaxFactory.PredefinedType(SyntaxFactory.Token(_kind)));

    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public new PredefinedTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new(_kind, (INamedTypeSymbol)compilation.GetNullable(_original), _symbol, true);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}
