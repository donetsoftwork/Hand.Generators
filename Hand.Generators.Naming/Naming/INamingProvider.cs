using Hand.Members;
using Hand.Methods;
using Microsoft.CodeAnalysis;

namespace Hand.Naming;

/// <summary>
/// 成员命名器
/// </summary>
public interface INamingProvider
{
    /// <summary>
    /// 是否已经命名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);
    /// <summary>
    /// 添加成员类型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="kind"></param>
    void AddKind(string name, MemberKind kind);
    /// <summary>
    /// 尝试获取成员类型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    bool TryGetMemberKind(string name, out MemberKind kind);
    /// <summary>
    /// 验证方法是否已经存在
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    bool Contains(MethodSignature method);
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
}
