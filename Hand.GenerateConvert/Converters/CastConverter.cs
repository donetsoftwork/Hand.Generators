using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 转化为目标类型
/// </summary>
/// <param name="targetType"></param>
public class CastConverter(TypeSyntax targetType)
     : IConverter
{
    #region 配置
    private readonly TypeSyntax _targetType = targetType;

    /// <summary>
    /// 目标类型
    /// </summary>
    public TypeSyntax TargetType
        => _targetType;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax Convert(ExpressionSyntax source)
        => SyntaxFactory.CastExpression(_targetType, source);
    ///// <inheritdoc />
    //public IConverter Nullable(ExpressionSyntax? defaultExpression)
    //    => this.CheckNull(defaultExpression);
    /// <summary>
    /// 转化为目标类型
    /// </summary>
    /// <param name="source">要转换的表达式</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>转换后的表达式</returns>
    public static ExpressionSyntax Convert(ExpressionSyntax source, TypeSyntax targetType)
         => SyntaxFactory.CastExpression(targetType, source);

    //public static IConverter Convert(Compilation compilation, Member sourceMember, Member targetMember)
    //{
    //    //var sourceType = sourceMember.Symbol;
    //    //var targetType = targetMember.Symbol;
    //    //compilation.HasImplicitConversion(sourceType, targetType);
    //    //var sourceExpression = SyntaxFactory.IdentifierName(sourceMember.Name);
    //    return Convert(compilation, sourceMember.MemberSymbol, targetMember.MemberSymbol);
    //}
    ///// <summary>
    ///// 转化为目标类型
    ///// </summary>
    ///// <param name="compilation"></param>
    ///// <param name="from">源类型</param>
    ///// <param name="to">目标类型</param>
    ///// <returns>转换后的表达式语句</returns>
    //public static IConverter Convert(Compilation compilation, MemberSymbolInfo from, MemberSymbolInfo to)
    //{
    //    var fromSymbol = from.Symbol;
    //    var toSymbol = to.Symbol;
    //    // 类型相同或存在隐式转换，直接返回源表达式
    //    if (SymbolTypeDescriptor.CheckEquals(fromSymbol, toSymbol) || compilation.HasImplicitConversion(fromSymbol, toSymbol))
    //        return NullCoalesceConverter.CheckNullable(new PassConverter(), true, false);

    //    //if (from.IsNullable())
    //    //{
    //    //    var underlyingType = from.TypeArguments[0];
    //    //    if (SymbolTypeDescriptor.CheckEquals(to, underlyingType))
    //    //    {
    //    //        var expression = SyntaxFactory.ConditionalExpression(source.IsNull(), SyntaxGenerator.DefaultLiteral, source);
    //    //        return SyntaxFactory.ExpressionStatement(expression);
    //    //        //return source.IsNull()
    //    //        //    .If()
    //    //        //    .Add(SyntaxGenerator.DefaultLiteral)
    //    //        //    .Else()
    //    //        //    .Add(source)
    //    //        //    .Build();
    //    //    }
    //    //}
    //    //else if (to.IsNullable())
    //    //{
    //    //    var underlyingType = to.TypeArguments[0];
    //    //    if (SymbolTypeDescriptor.CheckEquals(from, underlyingType))
    //    //        return SyntaxFactory.ExpressionStatement(source);
    //    //}
    //    //return SyntaxFactory.ExpressionStatement(source);
    //    return null;
    //}


    //new ImplicitTypeConverterResolver(),
    //    new ExplicitTypeConverterResolver(),
    //    new NestedTypeConverterResolver(),
    //    new EnumTypeConverterResolver(),
    //    new ArrayTypeConverterResolver(),
    //    new EnumerableTypeConverterResolver()
}
