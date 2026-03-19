using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand;

/// <summary>
/// 标记辅助类
/// </summary>
public class SyntaxAttributeHelper(SemanticModel semantic)
{
    #region 配置
    private readonly SemanticModel _semantic = semantic;
    #endregion
    /// <summary>
    /// 检查标记
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public static bool HasAttribute(MemberDeclarationSyntax syntax, string attributeName)
    {
        foreach (var list in syntax.AttributeLists)
        {
            foreach (var attribute in list.Attributes)
            {
                // 检查标记
                if (attribute.Name.ToFullString() == attributeName)
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 获取标记
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeSyntax> GetAttributesByType(MemberDeclarationSyntax syntax, string attributeName)
    {
        foreach (var list in syntax.AttributeLists)
        {
            foreach (var attribute in list.Attributes)
            {
                // 检查标记
                if (attribute.Name.ToFullString() == attributeName)
                    yield return attribute;
            }      
        }
    }
    #region GetArgumentValue
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="syntax"></param>
    /// <param name="attributeName"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public TValue? GetArgumentValue<TValue>(MemberDeclarationSyntax syntax, string attributeName, string name)
    {
        var attribute = GetAttributesByType(syntax, attributeName)
            .FirstOrDefault();
        if (attribute is null)
            return default;
        return GetArgumentValue<TValue>(attribute, name);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="syntax"></param>
    /// <param name="attributeName"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue? GetArgumentValue<TValue>(MemberDeclarationSyntax syntax, string attributeName, int index)
    {
        var attribute = GetAttributesByType(syntax, attributeName)
            .FirstOrDefault();
        if (attribute is null)
            return default;
        return GetArgumentValue<TValue>(attribute, index);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public TValue? GetArgumentValue<TValue>(AttributeSyntax attribute, string name)
        => GetArgumentValue<TValue>(GetArgument(attribute.ArgumentList, name));
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue? GetArgumentValue<TValue>(AttributeSyntax attribute, int index)
        => GetArgumentValue<TValue>(GetArgument(attribute.ArgumentList, index));    
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="argument"></param>
    /// <returns></returns>
    public TValue? GetArgumentValue<TValue>(AttributeArgumentSyntax? argument)
    {
        if (argument is null)
            return default;
        var expression = argument.Expression;
        if (expression is LiteralExpressionSyntax literal)
        {
            if (literal.Token.Value is TValue value)
                return value;
            return default;
        }
        var constant = _semantic.GetConstantValue(expression);
        if (constant.Value is TValue constantValue)
            return constantValue;
        return default;
    }
    #endregion
    #region GetArgument
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <param name="list"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static AttributeArgumentSyntax? GetArgument(AttributeArgumentListSyntax? list, string name)
    {
        if (list is null)
            return default;
        foreach (var item in list.Arguments)
        {
            var named = item.NameEquals;
            if (named is null)
                continue;
            if (named.Name.Identifier.Text == name)
                return item;
        }
        return default;
    }
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="attributeName"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static AttributeArgumentSyntax? GetArgument(MemberDeclarationSyntax syntax, string attributeName, string name)
    {
        var attribute = GetAttributesByType(syntax, attributeName)
            .FirstOrDefault();
        return GetArgument(attribute?.ArgumentList, name);
    }    
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static AttributeArgumentSyntax? GetArgument(AttributeArgumentListSyntax? list, int index)
    {
        if (list is null)
            return default;
        var arguments = list.Arguments;
        if (arguments.Count <= index)
            return default;
        return arguments[index];
    }
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="attributeName"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static AttributeArgumentSyntax? GetArgument(MemberDeclarationSyntax syntax, string attributeName, int index)
    {
        var attribute = GetAttributesByType(syntax, attributeName)
            .FirstOrDefault();
        return GetArgument(attribute?.ArgumentList, index);
    }
    #endregion
}
