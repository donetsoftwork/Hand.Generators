using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 默认值构造器
/// </summary>
public class DefaultExpressionBuilder
{
    #region 配置
    private static readonly ExpressionSyntax _suppress = SyntaxGenerator.DefaultLiteral.SuppressNull();
    #endregion
    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static ExpressionSyntax GetDefault(ITypeSymbolInfo info)
    {
        if (info.IsArray() || info.IsCollection())
            return SyntaxFactory.CollectionExpression();
        var symbol = info.Symbol;
        if (info.IsNullable || symbol.IsValueType)
            return SyntaxGenerator.DefaultLiteral;
        if (symbol.IsString())
            return SyntaxGenerator.Literal(string.Empty);

        if (info.IsEntity())
        {
            if (info is not EntityTypeInfo entityType)
                return SyntaxGenerator.DefaultLiteral;
            return symbol.ToSyntax().New([GetDefault(entityType.ElementInfo)]);
        }
        if (info.IsComplex() && symbol is INamedTypeSymbol namedType && SymbolReflection.GetEmptyConstructor(namedType) is not null)
            return symbol.ToSyntax().New();

        return _suppress;
    }
    /// <summary>
    /// 获取参数默认值
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static ExpressionSyntax GetTypedDefault(ITypeSymbolInfo info)
    {
        if (info.IsArray() && info is ArrayTypeInfo arrayInfo)
            return arrayInfo.Element.ToSyntax().EmptyArray();
        var symbol = info.Symbol;
        if (symbol.IsString())
            return SyntaxGenerator.Literal(string.Empty);
        if (info.IsEntity() && info is EntityTypeInfo entityType)
            return symbol.ToSyntax().New([GetTypedDefault(entityType.ElementInfo)]);
        if (info.IsComplex() && symbol is INamedTypeSymbol namedType && SymbolReflection.GetEmptyConstructor(namedType) is not null)
            return symbol.ToSyntax().New();
        if (info.IsNullable || symbol.IsValueType)
            return SyntaxFactory.DefaultExpression(symbol.ToSyntax());

        return SyntaxFactory.DefaultExpression(symbol.ToSyntax()).SuppressNull();
    }
    /// <summary>
    /// 获取参数默认值
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static ExpressionSyntax GetParameterDefault(ITypeSymbolInfo info)
    {
        if (info.IsNullable)
            return SyntaxGenerator.DefaultLiteral;
        if (info.Symbol.IsValueType)
            return SyntaxGenerator.DefaultLiteral;
        return _suppress;
    }
}
