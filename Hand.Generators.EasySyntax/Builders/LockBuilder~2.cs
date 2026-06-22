namespace Hand.Builders;

/// <summary>
/// lock
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class LockBuilder<TGrandpa, TParent>(TParent parent, LockBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly LockBuilder _original = original;
    /// <summary>
    /// 原始lock
    /// </summary>
    public LockBuilder Original
        => _original;
    #endregion

    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
}
