using Hand.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Members;

/// <summary>
/// 成员
/// </summary>
/// <param name="name">成员名</param>
/// <param name="symbolInfo">成员类型</param>
/// <param name="summary"></param>
/// <param name="element"></param>
public abstract class Member(string name, TypeSymbolInfo symbolInfo, Lazy<string> summary, Lazy<XmlElementSyntax?> element)
    : IEquatable<Member>
{
    /// <summary>
    /// 成员
    /// </summary>
    /// <param name="name"></param>
    /// <param name="symbolInfo"></param>
    /// <param name="summary"></param>
    /// <param name="element"></param>
    public Member(string name, TypeSymbolInfo symbolInfo, Func<string> summary, Func<XmlElementSyntax?> element)
        : this(name, symbolInfo, new Lazy<string>(summary), new Lazy<XmlElementSyntax?>(element))
    {
    }
    #region 配置
    private readonly string _name = name;
    /// <summary>
    /// 成员名
    /// </summary>
    public string Name
        => _name;
    /// <summary>
    /// 成员类型
    /// </summary>
    public abstract MemberKind Kind { get; }
    /// <summary>
    /// 成员类型信息
    /// </summary>
    protected readonly TypeSymbolInfo _symbolInfo = symbolInfo;
    /// <summary>
    /// 成员类型
    /// </summary>
    public TypeSymbolInfo SymbolInfo
        => _symbolInfo;
    private readonly Lazy<string> _summary = summary;
    /// <summary>
    /// 备注
    /// </summary>
    public string Summary
        => _summary.Value;
    private readonly Lazy<XmlElementSyntax?> _element = element;
    /// <summary>
    /// Xml备注
    /// </summary>
    public XmlElementSyntax? Element
        => _element.Value;
    #endregion

    /// <inheritdoc />
    public bool Equals(Member other)
        => string.Equals(_name, other._name, StringComparison.OrdinalIgnoreCase);
    ///// <summary>
    ///// 获取成员名
    ///// </summary>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public static IEnumerable<string> GetMemberNames(INamedTypeSymbol type)
    //{
    //    return type.GetMembers()
    //        .Select(static item => item.Name);
    //}
}
