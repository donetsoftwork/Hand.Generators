using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 语句收集器
/// </summary>
public abstract class StatementCollect
{
    #region 配置
    /// <summary>
    /// 语句
    /// </summary>
    protected readonly List<StatementSyntax> _statements = [];
    #endregion
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="statement"></param>
    protected internal virtual void AddCore(StatementSyntax statement)
        => _statements.Add(statement);
    /// <summary>
    /// 合并
    /// </summary>
    /// <param name="statements"></param>
    /// <returns></returns>
    public static StatementSyntax? Concat(List<StatementSyntax> statements)
    {
        return statements.Count switch
        {
            0 => null,
            1 => statements[0],
            _ => SyntaxFactory.Block(statements),
        };
    }
    /// <summary>
    /// 打包
    /// </summary>
    /// <param name="statements"></param>
    /// <returns></returns>
    public static StatementSyntax Block(List<StatementSyntax> statements)
    {
        return statements.Count switch
        {
            0 => SyntaxFactory.EmptyStatement(),
            1 => statements[0],
            _ => SyntaxFactory.Block(statements),
        };
    }
}
