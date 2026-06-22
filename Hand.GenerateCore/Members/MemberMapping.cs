using Microsoft.CodeAnalysis;

namespace Hand.Members;

/// <summary>
/// 成员映射
/// </summary>
/// <param name="name"></param>
/// <param name="memberSymbol"></param>
/// <param name="kind"></param>
/// <param name="source"></param>
public class MemberMapping(string name, MemberSymbolInfo memberSymbol, SymbolKind kind, Member? source = null)
    : MemberArgument(name, memberSymbol, kind)
{
    #region 配置
    /// <summary>
    /// 成员映射来源
    /// </summary>
    public Member? Source { get; set; } = source;
    #endregion
}
