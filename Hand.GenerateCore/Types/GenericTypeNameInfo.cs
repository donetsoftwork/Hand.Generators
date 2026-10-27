using Hand.Scopes;
using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Xml.Linq;

namespace Hand.Types;

/// <summary>
/// 泛型类型信息
/// </summary>
/// <param name="typeName"></param>
/// <param name="namespace"></param>
/// <param name="arguments"></param>
public class GenericTypeNameInfo(string typeName, NameSyntax @namespace, params ISyntaxDisplay<TypeSyntax>[] arguments)
    : TypeNameInfo(typeName, @namespace)
{
    #region 配置
    private readonly ISyntaxDisplay<TypeSyntax>[] _arguments = arguments;
    /// <summary>
    /// 泛型参数
    /// </summary>
    public ISyntaxDisplay<TypeSyntax>[] Arguments 
        => _arguments;
    #endregion

    /// <inheritdoc />
    public override TypeSyntax Display(SyntaxGenerator generator)
        => generator.GenericDisplay(_typeName, _namespace, Array.ConvertAll(_arguments, argument => argument.Display(generator)));
    /// <summary>
    /// List~element
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static GenericTypeNameInfo ListType(ISyntaxDisplay<TypeSyntax> element)
        => new("List", NamespaceScope.GenericCollections, element);
    /// <summary>
    /// 创建集合类型
    /// </summary>
    /// <param name="name">List\HashSet\SortedSet\IList\ISet等</param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static GenericTypeNameInfo Collection(string name, ISyntaxDisplay<TypeSyntax> element)
        => new(name, NamespaceScope.GenericCollections, element);
    /// <summary>
    /// 创建集合类型
    /// </summary>
    /// <param name="name">List\HashSet\SortedSet\Ilist\ISet等</param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static GenericTypeNameInfo Collection(string name, TypeSyntax element)
        => new(name, NamespaceScope.GenericCollections, new SyntaxWrapper<TypeSyntax>(element));
    /// <summary>
    /// Dictionary
    /// </summary>
    /// <param name="keyType"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static GenericTypeNameInfo Dictionary(ISyntaxDisplay<TypeSyntax> keyType, ISyntaxDisplay<TypeSyntax> valueType)
        => new("Dictionary", NamespaceScope.GenericCollections, keyType, valueType);
    /// <summary>
    /// 创建字典类型
    /// </summary>
    /// <param name="name">Dictionary\IDictionary等</param>
    /// <param name="keyType"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static GenericTypeNameInfo Dictionary(string name, ISyntaxDisplay<TypeSyntax> keyType, ISyntaxDisplay<TypeSyntax> valueType)
        => new(name, NamespaceScope.GenericCollections, keyType, valueType);
    /// <summary>
    /// 创建字典类型
    /// </summary>
    /// <param name="name">Dictionary\IDictionary等</param>
    /// <param name="keyType"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static GenericTypeNameInfo Dictionary(string name, TypeSyntax keyType, TypeSyntax valueType)
        => new(name, NamespaceScope.GenericCollections, new SyntaxWrapper<TypeSyntax>(keyType), new SyntaxWrapper<TypeSyntax>(valueType));
}
