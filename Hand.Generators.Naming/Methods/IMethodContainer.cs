using Microsoft.CodeAnalysis;

namespace Hand.Methods;

/// <summary>
/// 方法容器
/// </summary>
public interface IMethodContainer
{
    /// <summary>
    /// 验证方法是否已经存在
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    bool Contains(MethodSignature method);
    /// <summary>
    /// 验证方法是否已经存在
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    bool MethodContains(string name, params ITypeSymbol[] parameterSymbols);
    /// <summary>
    /// 方法重载数量
    /// </summary>
    /// <returns></returns>
    int MethodCount(string name);
    /// <summary>
    /// 尝试定义方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    bool TryDeclareMethod(string name, params ITypeSymbol[] parameterSymbols);
    /// <summary>
    /// 尝试定义方法
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    bool TryDeclareMethod(MethodSignature method);
    /// <summary>
    /// 验证构造函数是否已经存在
    /// </summary>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    bool ConstructorContains(params ITypeSymbol[] parameterSymbols);
    /// <summary>
    /// 构造函数数量
    /// </summary>
    /// <returns></returns>
    int ConstructorCount();
}
