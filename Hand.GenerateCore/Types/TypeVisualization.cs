using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 类型可见性信息
/// </summary>
/// <param name="info"></param>
/// <param name="isStatic"></param>
/// <param name="isInternal"></param>
public class TypeVisualization(TypeNameInfo info, bool isStatic, bool isInternal)
    : ISyntaxDisplay<TypeSyntax>
{
    #region 配置
    private readonly TypeNameInfo _info = info;
    private readonly bool _isStatic = isStatic;
    private readonly bool _isInternal = isInternal;

    /// <summary>
    /// 类型全名
    /// </summary>
    public string TypeName
        => _info.TypeName;
    /// <summary>
    /// 命名空间
    /// </summary>
    public NameSyntax Namespace
        => _info.Namespace;
    /// <summary>
    /// 完整类型名
    /// </summary>
    public string FullName
        => _info.FullName;
    /// <summary>
    /// 是否静态类
    /// </summary>
    public bool IsStatic 
        => _isStatic;
    /// <summary>
    /// 是否当前程序集可见
    /// </summary>
    public bool IsInternal 
        => _isInternal;
    #endregion

    /// <inheritdoc />
    public virtual TypeSyntax Display(SyntaxGenerator generator)
        => _info.Display(generator);

    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public static TypeVisualization GetInfo(INamedTypeSymbol symbol, bool isInternal = false)
        => new(new(symbol.Name, symbol.ContainingNamespace.ToName()), symbol.IsStatic, isInternal);
    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public static TypeVisualization GetExtensionInfo(INamedTypeSymbol symbol, bool isInternal = true)
        => new(new(symbol.Name + "Extensions", symbol.ContainingNamespace.ToName()), true, isInternal);
}
