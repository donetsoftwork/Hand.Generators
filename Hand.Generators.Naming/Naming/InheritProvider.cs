using Hand.Members;
using Hand.Methods;
using Microsoft.CodeAnalysis;

namespace Hand.Naming;

/// <summary>
/// 继承成员命名器
/// </summary>
/// <param name="original"></param>
/// <param name="base"></param>
public sealed class InheritProvider(INamingProvider original, INamingProvider @base)
    : INamingProvider
{
    #region 配置
    private readonly INamingProvider _original = original;
    private readonly INamingProvider _base = @base;
    /// <summary>
    /// 基类成员命名器
    /// </summary>
    public INamingProvider Base 
        => _base;
    /// <summary>
    /// 当前成员命名器
    /// </summary>
    public INamingProvider Original 
        => _original;
    #endregion
    #region NamingProvider
    /// <inheritdoc />
    public bool Contains(string name)
        => _original.Contains(name) || _base.Contains(name);
    /// <inheritdoc />
    public void AddKind(string name, MemberKind kind)
        => _original.AddKind(name, kind);
    /// <inheritdoc />
    public bool TryGetMemberKind(string name, out MemberKind kind)
        => _original.TryGetMemberKind(name, out kind) || _base.TryGetMemberKind(name, out kind);
    /// <inheritdoc />
    public bool Contains(MethodSignature method)
        => _original.Contains(method) || _base.Contains(method);
    /// <inheritdoc />
    public bool TryDeclareMethod(string name, params ITypeSymbol[] parameterSymbols)
        => !_base.Contains(name) && _original.TryDeclareMethod(name, parameterSymbols);
    /// <inheritdoc />
    public bool TryDeclareMethod(MethodSignature method)
        => !_base.Contains(method.Name) && _original.TryDeclareMethod(method);
    #endregion
    /// <summary>
    /// 尝试定义字段
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isNew"></param>
    /// <returns></returns>
    public bool TryDeclareField(string name, out bool isNew)
    {
        if (_original.TryDeclareField(name))
        {
            isNew = _base.Contains(name);
            return true;
        }
        isNew = false;
        return false;
    }
    /// <summary>
    /// 尝试定义属性
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isNew"></param>
    /// <returns></returns>
    public bool TryDeclareProperty(string name, out bool isNew)
    {
        if (_original.TryDeclareProperty(name))
        {
            isNew = _base.Contains(name);
            return true;
        }
        isNew = false;
        return false;
    }
}
