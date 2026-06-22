using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// for
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class ForBuilder<TGrandpa, TParent>(TParent parent, ForBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly ForBuilder _original = original;
    /// <summary>
    /// 原始for
    /// </summary>
    public ForBuilder Original
        => _original;
    #endregion
    #region Add
    /// <summary>
    /// 添加变量
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public ForBuilder<TGrandpa, TParent> AddVariable(VariableDeclaratorSyntax variable)
    {
        _original.AddVariable(variable);
        return this;
    }
    /// <summary>
    /// 添加初始化
    /// </summary>
    /// <param name="initializer"></param>
    /// <returns></returns>
    public ForBuilder<TGrandpa, TParent> AddInitializer(ExpressionSyntax initializer)
    {
        _original.AddInitializer(initializer);
        return this;
    }
    /// <summary>
    /// 添加自增加
    /// </summary>
    /// <param name="incrementor"></param>
    /// <returns></returns>
    public ForBuilder<TGrandpa, TParent> AddIncrementor(ExpressionSyntax incrementor)
    {
        _original.AddIncrementor(incrementor);
        return this;
    }
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
}
