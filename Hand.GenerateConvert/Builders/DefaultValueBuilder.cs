using Hand.Cache;
using Hand.Members;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 默认值构造器
/// </summary>
/// <param name="compilation"></param>
public class DefaultValueBuilder(Compilation compilation)
    : CacheFactoryBase<MemberSymbolInfo, ExpressionSyntax>()
{
    #region 配置
    private readonly Compilation _compilation = compilation;
    /// <summary>
    /// 编译信息
    /// </summary>
    public Compilation Compilation
        => _compilation;
    #endregion

    /// <inheritdoc />
    protected override ExpressionSyntax CreateNew(in MemberSymbolInfo key)
    {
        var category = key.Category;
        var symbol = key.Symbol;
        if (category.IsNullable() || symbol.IsValueType)
            return SyntaxGenerator.DefaultLiteral;
        if (symbol.IsString())
            return SyntaxGenerator.Literal(string.Empty);
        if (category.IsArray() || category.IsCollection())
            return SyntaxFactory.CollectionExpression();
        if (category.IsEntity())
            return SyntaxGenerator.New([Get(MemberSymbolInfo.Create(_compilation, key.ElementSymbol!))]);
        if (category.IsComplex() && SymbolReflection.GetEmptyConstructor(symbol) is not null)
            return SyntaxFactory.ImplicitObjectCreationExpression();

        return SyntaxGenerator.DefaultLiteral;
    }
}
