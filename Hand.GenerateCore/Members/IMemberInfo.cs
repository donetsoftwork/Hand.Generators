using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Hand.Members;

/// <summary>
/// 成员信息(字段、属性、参数)
/// </summary>
public interface IMemberInfo
{
    /// <summary>
    /// 成员类型
    /// </summary>
    MemberKind Kind { get; }
    /// <summary>
    /// 是否公开
    /// </summary>
    bool IsPublic { get; }
    /// <summary>
    /// 成员名
    /// </summary>
    string Name { get; }
    /// <summary>
    /// 成员符号信息
    /// </summary>
    ITypeSymbolInfo SymbolInfo { get; }
    /// <summary>
    /// 备注
    /// </summary>
    string Summary { get; }
    /// <summary>
    /// Xml备注
    /// </summary>
    XmlElementSyntax? XmlElement { get; }
    /// <summary>
    /// 获取特性标记
    /// </summary>
    /// <returns></returns>
    ImmutableArray<AttributeData> GetAttributes();
}