using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 作用域构造器
/// </summary>
public class ScopeBuilder(List<StatementSyntax> statements)
    : StatementBuilder(statements)
{
    /// <summary>
    /// 打包
    /// </summary>
    public BlockSyntax Block()
    {
        var statement = Build();
        if(statement is BlockSyntax block)
            return block;
        return SyntaxFactory.Block(statement);
    }
}