using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Hand.Members;

/// <summary>
/// 成员基类(字段、属性、参数和方法)
/// </summary>
/// <param name="name">成员名</param>
/// <param name="symbolInfo">成员类型</param>
public abstract class MemberBase(string name, ITypeSymbolInfo symbolInfo)
    : IMemberInfo
{
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
    /// <inheritdoc cref="SymbolInfo" path="/summary" />
    protected readonly ITypeSymbolInfo _symbolInfo = symbolInfo;
    /// <summary>
    /// 成员类型信息
    /// </summary>
    public ITypeSymbolInfo SymbolInfo
        => _symbolInfo;
    //private readonly Lazy<string> _summary = summary;
    /// <summary>
    /// 备注
    /// </summary>
    public abstract string Summary { get; }
    //private readonly Lazy<XmlElementSyntax?> _xmlElement = xmlElement;
    /// <summary>
    /// Xml备注
    /// </summary>
    public abstract XmlElementSyntax? XmlElement { get; }
    /// <inheritdoc />
    public abstract bool IsPublic { get; }
    /// <inheritdoc />
    public abstract ImmutableArray<AttributeData> GetAttributes();
    #endregion
}
