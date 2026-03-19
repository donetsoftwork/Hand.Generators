using Microsoft.CodeAnalysis;

namespace Hand.Symbols;

/// <summary>
/// 收集成员
/// </summary>
/// <typeparam name="TSymbol"></typeparam>
/// <param name="kind"></param>
public abstract class SymbolCollect<TSymbol>(SymbolKind kind)
    : ISymbolCollect
    where TSymbol : ISymbol
{
    private readonly SymbolKind _kind = kind;

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="syntax"></param>
    /// <returns></returns>
    public virtual bool Add(ISymbol syntax)
    {
        if (syntax.Kind == _kind && syntax is TSymbol member)
            return AddCore(member);
        return false;
    }
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="member"></param>
    protected abstract bool AddCore(TSymbol member);
}
