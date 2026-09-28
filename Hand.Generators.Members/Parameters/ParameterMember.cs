using Hand.Builders;
using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Hand.Parameters;

/// <summary>
/// 参数成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
public class ParameterMember(string name, IParameterSymbol original, ITypeSymbolInfo symbolInfo)
    : SymbolMember(name, original, symbolInfo), ISymbolMemberInfo
{
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.Parameter;
    /// <summary>
    /// 原始参数
    /// </summary>
    public new IParameterSymbol Original { get; } = original;
    /// <inheritdoc />
    public override bool IsPublic => true;

    /// <summary>
    /// 获取方法成员字典
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Dictionary<string, IMemberInfo> GetDictionary(TypeInfoBuilder typeSymbols, IMethodSymbol? method)
    {
        if (method == null)
            return new Dictionary<string, IMemberInfo>(StringComparer.OrdinalIgnoreCase);
        var parameters = method.Parameters;
        var members = new Dictionary<string, IMemberInfo>(parameters.Length, StringComparer.OrdinalIgnoreCase);
        foreach (var parameter in parameters)
        {
            var name = parameter.Name;
            members.Add(name, new ParameterMember(name, parameter, typeSymbols.Get(parameter.Type)));
        }
        return members;
    }
    /// <summary>
    /// 获取参数列表
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static List<ParameterMember> GetList(TypeInfoBuilder typeSymbols, IMethodSymbol? method)
    {
        if (method == null)
            return [];
        var parameters = method.Parameters;
        var members = new List<ParameterMember>(parameters.Length);
        foreach (var parameter in parameters)
        {
            members.Add(new ParameterMember(parameter.Name, parameter, typeSymbols.Get(parameter.Type)));
        }
        return members;
    }
}
