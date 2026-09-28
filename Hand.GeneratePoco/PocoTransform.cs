using Hand.Attributes;
using Hand.Builders;
using Hand.Cachers;
using Hand.Converters;
using Hand.Entities;
using Hand.Generators;
using Hand.Reflection;
using Hand.Transform;
using Hand.Types;
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
        if (context.TargetSymbol is not INamedTypeSymbol toSymbol)
            return null;
        var compilation = context.SemanticModel.Compilation;
        var typeCacher = new TypeInfoBuilder(compilation);
        if (typeCacher.Get(toSymbol) is not ComplexTypeInfo toInfo)
            return null;

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null) 
            return null;
        var fromSymbol = ConvertBuilder.CheckToSymbol(attribute);
        if (fromSymbol is null || fromSymbol.Equals(toSymbol, SymbolEqualityComparer.IncludeNullability))
            return null;
        if (typeCacher.Get(fromSymbol) is not ComplexTypeInfo fromInfo)
            return null;
        var isRecord = IsRecord(type);
        var initializer = CheckInitializeKind(attribute, CheckDefaultKind(toSymbol, isRecord));

        var convertBuilder = new ConvertBuilder(compilation, typeCacher, SystemConvertProvider.Create(compilation), new(compilation), []);
        if (isRecord && initializer == InitializeKind.Constructor)
            return new PocoRecordSource(type, convertBuilder, toInfo, fromInfo, attribute);
        if ((initializer & InitializeKind.Constructor) == InitializeKind.Constructor)
            return new PocoConstructorSource(type, convertBuilder, toInfo, fromInfo, attribute, initializer);
        if (initializer == InitializeKind.Field)
            return new PocoFieldSource(type, convertBuilder, toInfo, fromInfo, attribute);
        return new PocoPropertySource(type, convertBuilder, toInfo, fromInfo, attribute, initializer);
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
