using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Syntax;

/// <summary>
/// 泛型展示
/// </summary>
/// <param name="identifier"></param>
/// <param name="argumentTypes"></param>
public class GenericDisplay(SyntaxToken identifier, params ISyntaxDisplay<TypeSyntax>[] argumentTypes)
    : ISyntaxDisplay<GenericNameSyntax>
{
    #region 配置
    private readonly SyntaxToken _identifier = identifier;
    private readonly ISyntaxDisplay<TypeSyntax>[] _argumentTypes = argumentTypes;

    /// <summary>
    /// 泛型标识
    /// </summary>
    public SyntaxToken Identifier 
        => _identifier;
    /// <summary>
    /// 泛型参数
    /// </summary>
    public ISyntaxDisplay<TypeSyntax>[] ArgumentTypes 
        => _argumentTypes;
    #endregion

    /// <inheritdoc />
    public GenericNameSyntax Display(SyntaxGenerator generator)
    {
        var arguments = Array.ConvertAll(_argumentTypes, argumentType => argumentType.Display(generator));
        return SyntaxFactory.GenericName(_identifier, SyntaxFactory.TypeArgumentList([.. arguments]));
    }
}
