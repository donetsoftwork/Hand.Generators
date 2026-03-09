namespace Hand.Builders;

/// <summary>
/// 语句构造器基类
/// </summary>
public abstract class StatementBuilder<TParent>(TParent parent)
    : StatementCollect
{
    #region 配置
    /// <summary>
    /// 父对象
    /// </summary>
    protected readonly TParent _parent = parent;
    /// <summary>
    /// 父对象
    /// </summary>
    public TParent Parent 
        => _parent;
    #endregion
    /// <summary>
    /// 构造
    /// </summary>
    protected internal abstract TParent BuildCore();
}
