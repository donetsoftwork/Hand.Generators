using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.GenerateCachedProperty;

/// <summary>
/// 方法缓存源对象
/// </summary>
/// <param name="method"></param>
/// <param name="type"></param>
/// <param name="typeSymbol"></param>
/// <param name="propertyName"></param>
/// <param name="propertySymbol"></param>
/// <param name="isStatic"></param>
public class LazyMethodSource(MethodDeclarationSyntax method, TypeDeclarationSyntax type, INamedTypeSymbol typeSymbol, string? propertyName, INamedTypeSymbol propertySymbol, bool isStatic)
    : GenerateLazySource(type, typeSymbol, CheckPropertyName(propertyName, method.Identifier), propertySymbol, isStatic)
{
    private readonly MethodDeclarationSyntax _method = method;

    /// <inheritdoc />
    protected override ExpressionSyntax GetValueExpression()
    {
        if (CheckImMutable(_method.Modifiers))
        {
            var expression = GetReturnExpression(_method.ExpressionBody, _method.Body);
            if (expression is not null)
                return expression;
        }
        return _method.Identifier.ToIdentifierName().Invocation();
    }

}
