using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Arguments;

/// <summary>
/// 参数集合
/// </summary>
public interface IArgumentCollection
{
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="argument"></param>
    void Add(ArgumentSyntax argument);
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <returns></returns>
    IEnumerable<ArgumentSyntax> GetArguments();
}
