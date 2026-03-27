using Hand.Generators;
using Hand.Symbols;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hand.GenerateCachedProperty;

/// <summary>
/// 构造缓存转化器
/// </summary>

public class GenerateLazyTransform : IGeneratorTransform<GenerateLazySource>
{
    /// <inheritdoc />
    public GenerateLazySource? Transform(AttributeContext context, CancellationToken cancellation)
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
        var propertyName = GetPropertyNameByAttribute(context.Attributes, attributeType);
        GenerateLazySource? source = null;
        if (targetNode is PropertyDeclarationSyntax property)
        {
            var propertySymbol = semanticModel.GetDeclaredSymbol(property, cancellation);
            if (propertySymbol is not null && propertySymbol.Type is INamedTypeSymbol symbol)
                source = new LazyPropertySource(property, type, typeSymbol, propertyName, symbol, property.Modifiers.IsStatic());
        }
        else if (targetNode is MethodDeclarationSyntax method)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellation);
            if (methodSymbol is not null && methodSymbol.ReturnType is INamedTypeSymbol symbol)
                source = new LazyMethodSource(method, type, typeSymbol, propertyName, symbol, method.Modifiers.IsStatic());
        }
        // 判断是否已经存在同名属性
        // 不存在才返回
        if (source is not null && CheckSource(source, compilation))
            return source;
        return null;
    }
    /// <summary>
    /// 判断延迟缓存源对象是否合法
    /// </summary>
    /// <param name="source"></param>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static bool CheckSource(GenerateLazySource source, Compilation compilation)
    {
        var descriptor = new SymbolTypeBuilder()
            .WithProperty()
            .WithField()
            .Build(compilation, source.Symbol);
        // 存在同名属性不生成
        if(descriptor.GetProperty(source.PropertyName) is not null)
            return false;
        // 存在同名字段不生成
        if (descriptor.GetField(source.ValueName) is not null)
            return false;
        if (descriptor.GetField(source.StateName) is not null)
            return false;
        if (descriptor.GetField(source.LockName) is not null)
            return false;
        return true;
    }
    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="type"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static IPropertySymbol? GetProperty(INamedTypeSymbol type, string propertyName)
    {
        return new SymbolCollect<IPropertySymbol>(SymbolKind.Property)
                .Select(type.GetMembers())
                .FirstOrDefault(item => item.Name == propertyName);
    }
    /// <summary>
    /// 从Attribute配置中获取属性名
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>

    public static string? GetPropertyNameByAttribute(IEnumerable<AttributeData> attributes, INamedTypeSymbol attributeType)
    {
        var attribute = SymbolAttributeHelper.GetAttributesByType(attributes, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return null;
        return SymbolAttributeHelper.GetArgumentValue<string>(attribute, 0);
    }
    /// <summary>
    /// 获取类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static SymbolTypeDescriptor GetDescriptor(Compilation compilation, INamedTypeSymbol symbol)
    {
        // 提取字段、属性、构造函数、方法和运算符重载等信息
        var builder = new SymbolTypeBuilder()
            .WithProperty();
        return builder.Build(compilation, symbol);
    }
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly GenerateLazyTransform Instance = new();
}
