using Hand.Converters.Methods;
using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 转化一个Enumerable到另一个Enumerable
/// </summary>
/// <param name="method"></param>
public class LinqConverter(ISyntaxDisplay<SimpleNameSyntax> method)
     : MethodConverter(method)
{
    #region 配置
    /// <summary>
    /// using System.Linq
    /// </summary>
    public static readonly UsingDirectiveSyntax UsingLinq = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Linq"));
    /// <summary>
    /// ToList
    /// </summary>
    protected static readonly SyntaxWrapper<SimpleNameSyntax> _toList = new(SyntaxFactory.IdentifierName("ToList"));
    /// <summary>
    /// ToArray
    /// </summary>
    protected static readonly SyntaxWrapper<SimpleNameSyntax> _toArray = new(SyntaxFactory.IdentifierName("ToArray"));

    #endregion

    #region ToList
    /// <summary>
    /// ToList
    /// </summary>
    /// <returns></returns>
    public static LinqConverter ToList()
        => new(_toList);
    /// <summary>
    /// 泛型ToList
    /// </summary>
    /// <param name="argumentType"></param>
    /// <returns></returns>
    public static LinqConverter ToList(ISyntaxDisplay<TypeSyntax> argumentType)
        => new(new GenericDisplay(_toList.Original.Identifier, argumentType));
    #endregion
    #region ToArray
    /// <summary>
    /// ToArray
    /// </summary>
    /// <returns></returns>
    public static LinqConverter ToArray()
        => new(_toArray);
    /// <summary>
    /// 泛型ToArray
    /// </summary>
    /// <param name="argumentType"></param>
    /// <returns></returns>
    public static LinqConverter ToArray(ISyntaxDisplay<TypeSyntax> argumentType)
        => new(new GenericDisplay(_toArray.Original.Identifier, argumentType));
    #endregion
}
