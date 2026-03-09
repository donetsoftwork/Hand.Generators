namespace Hand.Builders;

/// <summary>
/// 代码块
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
public class BlockBuilder<TGrandpa, TParent>(TParent parent)
    : ScopeBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(Block(_statements));
        return _parent;
    }
}
