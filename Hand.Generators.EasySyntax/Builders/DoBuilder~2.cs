namespace Hand.Builders;

/// <summary>
/// do while
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class DoBuilder<TGrandpa, TParent>(TParent parent, DoBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly DoBuilder _original = original;
    /// <summary>
    /// 原始do while
    /// </summary>
    public DoBuilder Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
}
