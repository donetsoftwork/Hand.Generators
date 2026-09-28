using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 集合类型
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
/// <param name="elementInfo"></param>
/// <param name="isInterface"></param>
public sealed class CollectionTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, ITypeSymbolInfo elementInfo, bool isInterface)
    : ICollectionTypeInfo, INamedSymbolInfo, ISyntaxDisplay<TypeSyntax>
{
    #region 配置
    private readonly INamedTypeSymbol _original = original;
    private readonly INamedTypeSymbol _symbol = symbol;
    private readonly bool _isNullable = isNullable;
    private readonly ITypeSymbol _element = elementInfo.Original;
    private readonly ITypeSymbolInfo _elementInfo = elementInfo;
    private readonly bool _isInterface = isInterface;

    /// <inheritdoc />
    public INamedTypeSymbol Original
        => _original;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Original
        => Original;
    /// <inheritdoc />
    public INamedTypeSymbol Symbol
        => _symbol;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Symbol
        => Symbol;
    /// <inheritdoc />
    public bool IsNullable
        => _isNullable;

    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Collection;
    /// <inheritdoc />
    public string Summary
        => _elementInfo.Summary;
    /// <inheritdoc />
    public ITypeSymbol Element
        => _element;
    /// <inheritdoc />
    public ITypeSymbolInfo ElementInfo
        => _elementInfo;
    /// <summary>
    /// 是否接口
    /// </summary>
    public bool IsInterface 
        => _isInterface;
    #endregion
    /// <summary>
    /// 可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public CollectionTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, true, _elementInfo, _isInterface);
    /// <inheritdoc />
    public TypeSyntax Display(SyntaxGenerator generator)
        => CollectionDisplay(generator, _symbol.Name, _elementInfo).Nullable(_isNullable);
    /// <inheritdoc />
    public TypeOfExpressionSyntax TypeOf(SyntaxGenerator generator)
        => SyntaxFactory.TypeOfExpression(CollectionDisplay(generator, _symbol.Name, _elementInfo));

    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);

    /// <summary>
    /// 展示集合类型
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="collectionName"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static TypeSyntax CollectionDisplay(SyntaxGenerator generator, string collectionName, ITypeSymbolInfo element)
    {
        generator.Using("System.Collections.Generic");
        return SyntaxGenerator.Generic(collectionName, element.Display(generator));
    }
}