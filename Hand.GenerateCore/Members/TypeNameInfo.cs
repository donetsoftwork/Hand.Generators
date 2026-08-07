using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Members;

/// <summary>
/// 类型信息
/// </summary>
/// <param name="type"></param>
/// <param name="typeName"></param>
/// <param name="namespace"></param>
public class TypeNameInfo(TypeSyntax type, string typeName, string @namespace)
{
    /// <summary>
    /// 类型信息
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    public TypeNameInfo(string typeName, string @namespace)
        : this(SyntaxFactory.IdentifierName($"global::{@namespace}.{typeName}"), typeName, @namespace)
    {
    }
    #region 配置
    private readonly TypeSyntax _type = type;
    private readonly string _typeName = typeName;
    private readonly string _namespace = @namespace;

    /// <summary>
    /// 语法
    /// </summary>
    public TypeSyntax Type
        => _type;
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
    #endregion
    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeNameInfo GetInfo(INamedTypeSymbol symbol)
        => new(symbol.ToSyntax(), symbol.Name, symbol.ContainingNamespace.ToDisplayString());
    /// <summary>
    /// 获取扩展类信息
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeNameInfo GetExtensionInfo(INamedTypeSymbol symbol)
        => new(symbol.Name + "Extensions", symbol.ContainingNamespace.ToDisplayString());
}
