using Hand.Converters.Methods;
using Hand.Creational;
using Hand.Methods;
using Hand.Providers;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
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
/// <param name="isExtension"></param>
public class ConvertSourceInfo(SourceProvider provider, TypeNameInfo typeInfo, ConvertMethodInfo methodInfo, bool isPartial, bool isStatic, bool isExtension = true)
    : ICreator<ISyntaxConverter>
{
    #region 配置
    private readonly SourceProvider _provider = provider;
    private readonly TypeNameInfo _typeInfo = typeInfo;
    private readonly ConvertMethodInfo _methodInfo = methodInfo;
    private readonly bool _isPartial = isPartial;
    private readonly bool _isStatic = isStatic;
    private readonly bool _isExtension = isExtension;

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
    /// <summary>
    /// 是否扩展方法
    /// </summary>
    public bool IsExtension 
        => _isExtension;
    #endregion

    /// <summary>
    /// 获取方法转化器
    /// </summary>
    /// <returns></returns>
    public ISyntaxConverter Create()
    {
        if (_isExtension)
            return MethodConverter.CreateExtensionMethod(_typeInfo.Namespace, _methodInfo.Name);
        if (_isStatic)
            return StaticMethodConverter.Create(_typeInfo, SyntaxFactory.IdentifierName(_methodInfo.Name));
        return MethodConverter.Create(SyntaxFactory.IdentifierName(_methodInfo.Name));
    }
    /// <summary>
    /// 获取方法转化器
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static ISyntaxConverter GetConverter(IMethodSymbol method)
    {
        if (method.IsExtensionMethod)
            return MethodConverter.CreateExtensionMethod(method.ContainingType.ContainingNamespace.ToDisplayString(), method.Name);
        if (method.IsStatic)
            return StaticMethodConverter.Create(method.ContainingType, method.Name);

        return MethodConverter.Create(SyntaxFactory.IdentifierName(method.Name));
    }
}
