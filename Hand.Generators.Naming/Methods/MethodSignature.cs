using Microsoft.CodeAnalysis;

namespace Hand.Methods;

/// <summary>
/// 方法签名
/// </summary>
/// <param name="name"></param>
/// <param name="parameterSymbols"></param>
public sealed class MethodSignature(string name, params ITypeSymbol[] parameterSymbols)
{
    #region 配置
    private readonly ITypeSymbol[] _parameterSymbols = parameterSymbols;
    private readonly int _parameterCount = parameterSymbols.Length;

    /// <inheritdoc />
    public string Name { get; } = name;

    /// <inheritdoc />
    public int ParameterCount => _parameterCount;
    /// <inheritdoc />
    public ITypeSymbol[] ParameterSymbols => _parameterSymbols;
    #endregion

    /// <inheritdoc />
    public bool VerifyParameter(int parameterCount, params ITypeSymbol[] parameterSymbols)
    {
        if (parameterCount == _parameterCount)
        {
            for (int i = 0; i < parameterCount; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(parameterSymbols[i], _parameterSymbols[i]))
                    return false;
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 构造方法签名
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static MethodSignature Create(IMethodSymbol method)
    {
        var parameters = method.Parameters;
        var count = parameters.Length;
        var parameterSymbols = new ITypeSymbol[count];
        for (int i = 0; i < count; i++)
            parameterSymbols[i] = parameters[i].Type;
        return new(method.Name, parameterSymbols);
    }
}
