using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 子作用域构造器
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="statements"></param>
public abstract class ScopeBuilder<TGrandpa, TParent>(TParent parent, List<StatementSyntax> statements)
    : StatementBuilder<TParent>(parent, statements)
    where TParent : StatementBuilder<TGrandpa>
{
    /// <summary>
    /// 当前作用域完成
    /// </summary>
    /// <returns></returns>
    public TParent End()
    {
        BuildCore();
        return _parent;
    }
}
