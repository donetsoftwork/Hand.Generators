using Hand.Cache;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 默认值构造器
/// </summary>
/// <param name="compilation"></param>
public class DefaultExpressionBuilder(Compilation compilation)
    : CacheFactoryBase<TypeSymbolInfo, ExpressionSyntax>()
{
    #region 配置
    private static readonly ExpressionSyntax _suppress = SyntaxGenerator.DefaultLiteral.SuppressNull();
    private readonly Compilation _compilation = compilation;
    /// <summary>
    /// 编译信息
    /// </summary>
    public Compilation Compilation
        => _compilation;
    #endregion

    /// <inheritdoc />
    protected override ExpressionSyntax CreateNew(in TypeSymbolInfo key)
        => Default(key, _compilation);
    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <param name="info"></param>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static ExpressionSyntax Default(TypeSymbolInfo info, Compilation compilation)
    {
        var kind = info.Kind;
        var symbol = info.Symbol;
        if (kind.IsNullable() || symbol.IsValueType)
            return SyntaxGenerator.DefaultLiteral;
        if (symbol.IsString())
            return SyntaxGenerator.Literal(string.Empty);
        if (kind.IsArray() || kind.IsCollection())
            return SyntaxFactory.CollectionExpression();
        if (kind.IsEntity())
        {
            var element = TypeSymbolInfo.Create(compilation, info.Element!);
            if(element is null)
                return SyntaxGenerator.DefaultLiteral;
            return symbol.ToSyntax().New([Default(element, compilation)]);
        }
        if (kind.IsComplex() && SymbolReflection.GetEmptyConstructor(symbol) is not null)
            return symbol.ToSyntax().New();

        return _suppress;
    }
}
