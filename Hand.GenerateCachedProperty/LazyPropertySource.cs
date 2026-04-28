using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.GenerateCachedProperty;

/// <summary>
/// 属性缓存源对象
/// </summary>
/// <param name="property"></param>
/// <param name="type"></param>
/// <param name="typeSymbol"></param>
/// <param name="propertyName"></param>
/// <param name="propertySymbol"></param>
/// <param name="isStatic"></param>
public class LazyPropertySource(PropertyDeclarationSyntax property, TypeDeclarationSyntax type, INamedTypeSymbol typeSymbol, string? propertyName, INamedTypeSymbol propertySymbol, bool isStatic)
    : GenerateLazySource(type, typeSymbol, CheckPropertyName(propertyName, property.Identifier), propertySymbol, isStatic)
{
    #region 配置
    private readonly PropertyDeclarationSyntax _property = property;
    #endregion

    /// <inheritdoc />
    protected override ExpressionSyntax GetValueExpression()
    {
        if(CheckImMutable(_property.Modifiers))
        {
            var accessor = _property.GetAccessor();
            if(accessor is not null && CheckImMutable(accessor.Modifiers))
            {
                var expression = GetReturnExpression(accessor.ExpressionBody, accessor.Body);
                if (expression is not null)
                    return expression;
            }
        }

        return _property.Identifier.ToIdentifierName();
    }
}
