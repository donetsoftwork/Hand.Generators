using Hand.Attributes;
using Hand.Generators;
using Hand.Naming;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;

namespace Hand.GenerateCachedProperty;

/// <summary>
/// 构造缓存转化器
/// </summary>

public class GenerateLazyTransform : IGeneratorTransform<GenerateLazySource>
{
    /// <inheritdoc />
    public GenerateLazySource? Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
            return null;
        var targetNode = context.TargetNode;
        if (targetNode.Parent is not TypeDeclarationSyntax type || !type.Modifiers.IsPartial())
            return null;
        var semanticModel = context.SemanticModel;
        var typeSymbol = semanticModel.GetDeclaredSymbol(type, cancellation);
        if (typeSymbol is null) 
            return null;
        var compilation = semanticModel.Compilation;
        var attributeType = compilation.GetTypeByMetadataName(GenerateLazyGenerator.Attribute);
        if (attributeType is null) 
            return null;
        var attribute = SymbolAttributeHelper.GetAttributesByType(context.Attributes, attributeType)
            .FirstOrDefault();
        var propertyName = GetPropertyNameByAttribute(attribute);
        var lockType = GetLockTypeByAttribute(attribute);
        GenerateLazySource? source = null;
        if (targetNode is PropertyDeclarationSyntax property)
        {
            var propertySymbol = semanticModel.GetDeclaredSymbol(property, cancellation);
            if (propertySymbol is not null && propertySymbol.Type is INamedTypeSymbol symbol)
                source = new LazyPropertySource(property, type, typeSymbol, propertyName, symbol, property.Modifiers.IsStatic(), lockType);
        }
        else if (targetNode is MethodDeclarationSyntax method)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellation);
            if (methodSymbol is not null && methodSymbol.ReturnType is INamedTypeSymbol symbol)
                source = new LazyMethodSource(method, type, typeSymbol, propertyName, symbol, method.Modifiers.IsStatic(), lockType);
        }
        // 判断是否已经存在同名属性
        // 不存在才返回
        if (source is not null && CheckSource(source))
            return source;
        return null;
    }
    /// <summary>
    /// 判断延迟缓存源对象是否合法
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool CheckSource(GenerateLazySource source)
    {
        var provider = TypedProvider.Create(source.Symbol);
        
        // 存在同名属性不生成
        if(provider.Contains(source.PropertyName))
            return false;
        // 存在同名字段不生成
        if (provider.Contains(source.ValueName))
            return false;
        if (provider.Contains(source.StateName))
            return false;
        if (provider.Contains(source.LockName))
            return false;
        return true;
    }
    /// <summary>
    /// 从Attribute配置中获取属性名
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static string? GetPropertyNameByAttribute(AttributeData attribute)
    {
        var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, 0);
        if (argument == null) 
            return null;
        return argument.Value.GetPrimitive<string>();
    }
    /// <summary>
    /// 从Attribute配置中获取锁类型
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static TypeSyntax GetLockTypeByAttribute(AttributeData attribute)
    {
        var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, "LockType");
        if (argument is not null)
        {
            var lockTypeName = argument.Value.GetPrimitive<string>();
            if (!string.IsNullOrWhiteSpace(lockTypeName))
                return SyntaxFactory.IdentifierName(lockTypeName);
        }
        if (SyntaxGenerator.FrameworkMajorVersion >= 9)
            return SyntaxGenerator.LockType;
        return SyntaxGenerator.ObjectType;
    }
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly GenerateLazyTransform Instance = new();
}
