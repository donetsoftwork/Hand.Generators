using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 类型信息
/// </summary>
/// <param name="typeName"></param>
/// <param name="namespace"></param>
public class TypeNameInfo(string typeName, NameSyntax @namespace)
    : ISyntaxDisplay<TypeSyntax>
{
    #region 配置
    /// <inheritdoc cref="TypeName" path="/summary" />
    protected readonly string _typeName = typeName;
    /// <inheritdoc cref="Namespace" path="/summary" />
    protected readonly NameSyntax _namespace = @namespace;
    /// <summary>
    /// 类型全名
    /// </summary>
    public string TypeName
        => _typeName;
    /// <summary>
    /// 命名空间
    /// </summary>
    public NameSyntax Namespace
        => _namespace;
    /// <summary>
    /// 完整类型名
    /// </summary>
    public string FullName
        => CheckFullName(_namespace.ToFullString(), _typeName);
    #endregion

    /// <inheritdoc />
    public virtual TypeSyntax Display(SyntaxGenerator generator)
        => generator.Display(_typeName, _namespace);


    /// <summary>
    /// 获取完整类型名
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static string CheckFullName(string @namespace, string typeName)
        => string.IsNullOrWhiteSpace(@namespace) ? typeName : $"{@namespace}.{typeName}";
    /// <summary>
    /// 获取完整类型名
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static string CheckFullName(INamespaceSymbol @namespace, string typeName)
        => @namespace.IsGlobalNamespace ? typeName : $"{@namespace.ToDisplayString()}.{typeName}";

    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeNameInfo GetInfo(INamedTypeSymbol symbol)
        => new(symbol.Name, symbol.ContainingNamespace.ToName());
}
