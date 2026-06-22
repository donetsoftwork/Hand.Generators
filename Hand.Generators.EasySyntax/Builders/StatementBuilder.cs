using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 构造器
/// </summary>
public class StatementBuilder(List<StatementSyntax> statements)
    : StatementCollect(statements)
{
    /// <summary>
    /// 构造
    /// </summary>
    public virtual StatementSyntax Build()
        => Block(_statements);
}
