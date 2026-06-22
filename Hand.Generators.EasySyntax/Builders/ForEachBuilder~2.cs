namespace Hand.Builders;

/// <summary>
/// foreach
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class ForEachBuilder<TGrandpa, TParent>(TParent parent, ForEachBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly ForEachBuilder _original = original;
    /// <summary>
    /// 原始for
    /// </summary>
    public ForEachBuilder Original
        => _original;
    #endregion

    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
}
