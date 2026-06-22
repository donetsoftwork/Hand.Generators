namespace Hand.Builders;

/// <summary>
/// Else
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class ElseBuilder<TGrandpa, TParent>(TParent parent, ElseBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly ElseBuilder _original = original;
    /// <summary>
    /// 原始Else
    /// </summary>
    public ElseBuilder Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.BuildCurrent());
        return _parent;
    }
}
