using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 类型信息
/// </summary>
/// <param name="typeName"></param>
/// <param name="namespace"></param>
/// <param name="isStatic"></param>
/// <param name="isInternal"></param>
public class TypeNameInfo(string typeName, string @namespace, bool isStatic, bool isInternal)
    : ISyntaxDisplay<TypeSyntax>
{
    #region 配置
    private readonly string _typeName = typeName;
    private readonly string _namespace = @namespace;
    private readonly bool _isStatic = isStatic;
    private readonly bool _isInternal = isInternal;

    /// <summary>
    /// 类型全名
    /// </summary>
    public string TypeName
        => _typeName;
    /// <summary>
    /// 命名空间
    /// </summary>
    public string Namespace
        => _namespace;
    /// <summary>
    /// 完整类型名
    /// </summary>
    public string FullName
        => $"{Namespace}.{TypeName}";
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
        => generator.Display(_typeName, _namespace);

    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public static TypeNameInfo GetInfo(INamedTypeSymbol symbol, bool isInternal = false)
        => new(symbol.Name, symbol.ContainingNamespace.ToDisplayString(), symbol.IsStatic, isInternal);
    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public static TypeNameInfo GetExtensionInfo(INamedTypeSymbol symbol, bool isInternal = true)
        => new(symbol.Name + "Extensions", symbol.ContainingNamespace.ToDisplayString(), true, isInternal);
}
