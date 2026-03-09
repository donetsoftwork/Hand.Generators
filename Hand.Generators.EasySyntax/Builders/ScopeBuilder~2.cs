namespace Hand.Builders;

/// <summary>
/// 子作用构造器
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
public abstract class ScopeBuilder<TGrandpa, TParent>(TParent parent)
    : StatementBuilder<TParent>(parent)
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
