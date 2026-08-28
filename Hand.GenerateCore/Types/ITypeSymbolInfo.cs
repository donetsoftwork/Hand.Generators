using Hand.Reflection;
using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 类型信息
/// </summary>
public interface ITypeSymbolInfo
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
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    ITypeSymbolInfo GetNullable(Compilation compilation);
}
