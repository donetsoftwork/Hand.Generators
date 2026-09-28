using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 函数构造器
/// </summary>
/// <param name="method"></param>
/// <param name="parameters"></param>
public class MethodBodyBuilder<TMethod>(TMethod method, List<ParameterSyntax> parameters)
    : BodyBuilder<TMethod>(method)
    where TMethod : BaseMethodDeclarationSyntax
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public MethodBodyBuilder(TMethod method)
        : this(method, [])
    {
    }
    /// <summary>
    /// 参数列表
    /// </summary>
    private List<ParameterSyntax> _parameters = parameters;

    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public MethodBodyBuilder<TMethod> AddParameter(ParameterSyntax parameter)
    {
        _parameters.Add(parameter);
        return this;
    }

    /// <inheritdoc />
    protected internal override TMethod BuildCore()
        => (TMethod)_parent.AddParameterListParameters([.. _parameters]).WithBody(BuildBody());
}
