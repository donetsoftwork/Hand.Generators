using Hand.Converters;
using Hand.Creational;
using Hand.Providers;
using Microsoft.CodeAnalysis.CSharp;

namespace Hand.Members;

/// <summary>
/// 生成源信息
/// </summary>
/// <param name="provider"></param>
/// <param name="typeInfo"></param>
/// <param name="methodInfo"></param>
/// <param name="isPartial"></param>
/// <param name="isStatic"></param>
public class ConvertSourceInfo(SourceProvider provider, TypeNameInfo typeInfo, ConvertMethodInfo methodInfo, bool isPartial, bool isStatic)
    : ICreator<IConverter>
{
    #region 配置
    private readonly SourceProvider _provider = provider;
    private readonly TypeNameInfo _typeInfo = typeInfo;
    private readonly ConvertMethodInfo _methodInfo = methodInfo;
    private readonly bool _isPartial = isPartial;
    private readonly bool _isStatic = isStatic;

    /// <summary>
    /// 生成源提供者
    /// </summary>
    public SourceProvider Provider 
        => _provider;
    /// <summary>
    /// 类型信息
    /// </summary>
    public TypeNameInfo TypeInfo 
        => _typeInfo;
    /// <summary>
    /// 函数信息
    /// </summary>
    public ConvertMethodInfo MethodInfo 
        => _methodInfo;
    /// <summary>
    /// 是否静态方法
    /// </summary>
    public bool IsStatic 
        => _isStatic;
    /// <summary>
    /// 是否部分类
    /// </summary>
    public bool IsPartial 
        => _isPartial;
    #endregion
    /// <summary>
    /// 获取方法转化器
    /// </summary>
    /// <returns></returns>
    public IConverter Create()
    {
        if (_isStatic)
            return new StaticMethodConverter(_typeInfo.Type.Access(_methodInfo.Name));
        return new InstanceMethodConverter(SyntaxFactory.IdentifierName(_methodInfo.Name));
    }
}
