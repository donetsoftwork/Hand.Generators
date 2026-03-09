using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// Else
/// </summary>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="if"></param>
public class ElseBuilder<TGrandpa, TParent>(TParent parent, IfBuilder<TGrandpa, TParent> @if)
    : ScopeBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly IfBuilder<TGrandpa, TParent> _if = @if;

    public IfBuilder<TGrandpa, TParent> If
        => _if;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(BuildCurrent());
        return _parent;
    }
    /// <summary>
    /// 构造当前语句
    /// </summary>
    /// <returns></returns>
    protected internal IfStatementSyntax BuildCurrent()
    {
        var @if = _if.BuildCurrent();
        var statement = Concat(_statements);
        if(statement is null)
            return @if;        
        return @if.WithElse(SyntaxFactory.ElseClause(statement));
    }
}
