using Microsoft.CodeAnalysis;

namespace Hand.Members;

/// <summary>
/// 符号成员(字段、属性、参数)
/// </summary>
public interface ISymbolMemberInfo : IMemberInfo
{
    /// <summary>
    /// 原始成员
    /// </summary>
    ISymbol Original { get; }
}
