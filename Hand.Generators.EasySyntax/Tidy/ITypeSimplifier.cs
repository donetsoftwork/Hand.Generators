using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Tidy;

/// <summary>
/// 类型简化器
/// </summary>
public interface ITypeSimplifier
{
    /// <summary>
    /// 简化
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    TypeSyntax Simplify(TypeSyntax type);
}
