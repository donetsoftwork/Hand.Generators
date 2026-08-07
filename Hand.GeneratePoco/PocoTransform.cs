using Hand.Builders;
using Hand.Cachers;
using Hand.Converters;
using Hand.Entities;
using Hand.Generators;
using Hand.Reflection;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hand.GeneratePoco;

/// <summary>
/// 转化
/// </summary>
public class PocoTransform : IGeneratorTransform<PocoSource>
{
    /// <inheritdoc />
    public PocoSource? Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
            return null;
        if (context.TargetNode is not TypeDeclarationSyntax type)
            return null;
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
            return null;
        var compilation = context.SemanticModel.Compilation;
        var typeCacher = new TypeSymbolCacher(compilation);
        var typeInfo = typeCacher.Get(typeSymbol);
        if (typeInfo is null || typeInfo.Kind != TypeSymbolKind.Complex)
            return null;

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null) 
            return null;
        var sourseSymbol = ConvertBuilder.CheckToSymbol(attribute);
        if (sourseSymbol is null || sourseSymbol.Equals(typeSymbol, SymbolEqualityComparer.IncludeNullability))
            return null;
        var sourseInfo = typeCacher.Get(sourseSymbol);
        if (sourseInfo is null || sourseInfo.Kind != TypeSymbolKind.Complex)
            return null;
        var isRecord = IsRecord(type);
        var initializer = CheckInitializeKind(attribute, CheckDefaultKind(typeSymbol, isRecord));

        var convertBuilder = new ConvertBuilder(compilation, typeCacher, SystemConvertProvider.Create(compilation), new(compilation), []);
        if (isRecord && initializer == InitializeKind.Constructor)
            return new PocoRecordSource(type, convertBuilder, typeInfo, sourseInfo, attribute);
        if ((initializer & InitializeKind.Constructor) == InitializeKind.Constructor)
            return new PocoConstructorSource(type, convertBuilder, typeInfo, sourseInfo, attribute, initializer);
        if (initializer == InitializeKind.Field)
            return new PocoFieldSource(type, convertBuilder, typeInfo, sourseInfo, attribute);
        return new PocoPropertySource(type, convertBuilder, typeInfo, sourseInfo, attribute, initializer);
    }
    /// <summary>
    /// 是否记录类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRecord(TypeDeclarationSyntax type)
        => type.IsKind(SyntaxKind.RecordDeclaration) || type.IsKind(SyntaxKind.RecordStructDeclaration);
    /// <summary>
    /// 解析初始化类型
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static InitializeKind CheckInitializeKind(AttributeData attribute, InitializeKind defaultValue)
    {
        var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, "Initializer");
        if (argument is null)
            return defaultValue;
        return argument.Value.GetEnum(defaultValue);
    }
    /// <summary>
    /// 检查默认初始化类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="isRecord"></param>
    /// <returns></returns>
    public static InitializeKind CheckDefaultKind(INamedTypeSymbol symbol, bool isRecord)
    {
        var hasConstructor = SymbolReflection.GetConstructors(symbol, true)
            .Any(c => !c.IsStatic);
        if (hasConstructor)
            return InitializeKind.Property;
        if (isRecord)
            return InitializeKind.Constructor;
        return InitializeKind.Property;
    }    
    ///// <summary>
    ///// 单例
    ///// </summary>
    //public static readonly PocoTransform Instance = new();
}
