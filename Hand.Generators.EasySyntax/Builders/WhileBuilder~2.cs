namespace Hand.Builders;

/// <summary>
/// while
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class WhileBuilder<TGrandpa, TParent>(TParent parent, WhileBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly WhileBuilder _original = original;
    /// <summary>
    /// 原始While
    /// </summary>
    public WhileBuilder Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
}
