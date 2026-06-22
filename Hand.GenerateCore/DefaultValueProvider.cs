using Hand.Members;
using Hand.Symbols;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand;

/// <summary>
/// 默认值提供者
/// </summary>
public class DefaultValueProvider
{
    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static ExpressionSyntax GetDefault(MemberSymbolInfo info)
    {
        var category = info.Category;
        var symbol = info.Symbol;
        if (category.IsNullable() || symbol.IsValueType)
            return SyntaxGenerator.DefaultLiteral;
        if (category.IsNullable())
            return SyntaxFactory.CollectionExpression();
        if (category.IsComplex() && SymbolReflection.GetEmptyConstructor(symbol) is not null)
            return SyntaxFactory.ImplicitObjectCreationExpression();

        return SyntaxGenerator.DefaultLiteral;
    }
}
