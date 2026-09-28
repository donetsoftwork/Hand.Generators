using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 类型信息
/// </summary>
public interface ITypeSymbolInfo : ISyntaxDisplay<TypeSyntax>
{
    /// <summary>
    /// 原始类型
    /// </summary>
    ITypeSymbol Original { get; }
    /// <summary>
    /// 类型
    /// </summary>
    ITypeSymbol Symbol { get; }
    /// <summary>
    /// 成员类别
    /// </summary>
    TypeSymbolKind Kind { get; }
    /// <summary>
    /// 是否可空
    /// </summary>
    bool IsNullable { get; }
    /// <summary>
    /// 备注
    /// </summary>
    string Summary { get; }
    /// <summary>
    /// TypeOf表达式
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    TypeOfExpressionSyntax TypeOf(SyntaxGenerator generator);
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    ITypeSymbolInfo GetNullable(Compilation compilation);
}
