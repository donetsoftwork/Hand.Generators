using Hand.Builders;
using Hand.Enumerations.Builders;
using Hand.Enumerations.Services;
using Hand.Enumerations.Sources;
using Hand.Enums.Builders;
using Hand.GenerateEnumeration.Sources;
using Hand.Generators;
using Hand.Naming;
using Hand.Reflection;
using Hand.Sources;
using Hand.Syntax;
using Hand.Transform;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;

namespace Hand.GenerateEnumeration;

/// <summary>
/// 枚举类源信息转化
/// </summary>
public class EnumerationTransform : IGeneratorTransform<IGeneratorSource>
{
    /// <inheritdoc />
    public IGeneratorSource? Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        if (cancellation.IsCancellationRequested)
            return null;
        if (context.TargetNode is not TypeDeclarationSyntax type)
            return null;
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol || typeSymbol.IsAbstract || typeSymbol.TypeKind == TypeKind.Interface)
            return null;
        var compilation = context.SemanticModel.Compilation;
        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null)
            return null;
        var sourseSymbol = ConvertBuilder.CheckToSymbol(attribute);
        if (sourseSymbol is null)
            return null;
        var typeCacher = new TypeInfoBuilder(compilation);
        var sourseInfo = typeCacher.Get(sourseSymbol);
        var typeInfo = typeCacher.Get(typeSymbol);
        if (sourseInfo.Kind == TypeSymbolKind.Enum)
        {
            return typeInfo.Kind switch
            {
                TypeSymbolKind.Enumeration => CreateEnumSource(compilation, type, (EnumTypeInfo)sourseInfo, (EnumerationTypeInfo)typeInfo),
                TypeSymbolKind.Complex => CreateComplexSource(compilation, type, (EnumTypeInfo)sourseInfo, (ComplexTypeInfo)typeInfo),
                _ => null,
            };
        }
        else if (sourseInfo.Kind == TypeSymbolKind.Enumeration && typeInfo.Kind == TypeSymbolKind.Complex)
        {
            return CreateProviderSource(compilation, type, (EnumerationTypeInfo)sourseInfo, (ComplexTypeInfo)typeInfo);
        }

        return null;
    }
    /// <summary>
    /// 生成枚举提供器
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <param name="sourseInfo"></param>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    public EnumerationProviderSource? CreateProviderSource(Compilation compilation, TypeDeclarationSyntax type, EnumerationTypeInfo sourseInfo, ComplexTypeInfo typeInfo)
    {
        var symbol = typeInfo.Symbol;
        var providerNaming = CompositeProvider.DeclaredOnly(symbol);
        // 不能有构造函数
        if (providerNaming.ConstructorCount() > 0)
            return null;
        var sourseSymbol = sourseInfo.Symbol;
        var enumerationName = sourseSymbol.Name;
        var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
        var baseSymbol = symbol.BaseType;
        if(baseSymbol is null || baseSymbol.IsObject())
        {
            if (sourseInfo.IsFlag)
            {
                var genericFlagBase = EnumerationProviderBuilder.GetFlagGenericBase(compilation);
                if (genericFlagBase is null)
                    return null;
                var baseNaming = CompositeProvider.DeclaredOnly(genericFlagBase.Construct(sourseSymbol));
                var providerName = type.Identifier.ValueText;
                var naming = new InheritProvider(providerNaming.Typed(providerName), baseNaming);
                var providerBaseType = new GenericTypeNameInfo("FlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var generator = SyntaxGenerator.Clone(type);
                generator.AddBaseType(providerBaseType.Display(generator));
                return EnumerationProviderSource.CreateFlag(generator, sourseSymbol, naming, enumerationName, providerName);
            }
            else
            {
                var genericBase = EnumerationProviderBuilder.GetGenericBase(compilation);
                if (genericBase is null)
                    return null;
                var baseNaming = CompositeProvider.DeclaredOnly(genericBase.Construct(sourseSymbol));
                var providerName = type.Identifier.ValueText;
                var naming = new InheritProvider(providerNaming.Typed(providerName), baseNaming);
                var providerBaseType = new GenericTypeNameInfo("EnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var generator = SyntaxGenerator.Clone(type);
                generator.AddBaseType(providerBaseType.Display(generator));
                return EnumerationProviderSource.Create(generator, sourseSymbol, naming, enumerationName, providerName);
            }
        }
        var flagGenericInterface = EnumerationProviderBuilder.GetFlagGenericInterface(compilation);
        if (flagGenericInterface is null)
            return null;
        if(baseSymbol.IsInterface(flagGenericInterface.Construct(sourseSymbol)))
        {
            // 非位标记枚举类不能生成位标记枚举提供器
            if (!sourseInfo.IsFlag)
                return null;
            var baseNaming = CompositeProvider.DeclaredOnly(baseSymbol);
            var providerName = type.Identifier.ValueText;
            var naming = new InheritProvider(providerNaming.Typed(providerName), baseNaming);
            var generator = SyntaxGenerator.Clone(type);
            return EnumerationProviderSource.CreateFlag(generator, sourseSymbol, naming, enumerationName, providerName);
        }
        var genericInterface = EnumerationProviderBuilder.GetGenericInterface(compilation);
        if (genericInterface is null)
            return null;
        if (baseSymbol.IsInterface(genericInterface.Construct(sourseSymbol)))
        {
            var baseNaming = CompositeProvider.DeclaredOnly(baseSymbol);
            var providerName = type.Identifier.ValueText;
            var naming = new InheritProvider(providerNaming.Typed(providerName), baseNaming);
            var generator = SyntaxGenerator.Clone(type);
            return EnumerationProviderSource.Create(generator, sourseSymbol, naming, enumerationName, providerName);
        }

        return null;
    }
    /// <summary>
    /// 按枚举类型生成枚举类
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <param name="sourseInfo"></param>
    /// <param name="enumerationInfo"></param>
    /// <returns></returns>
    public IGeneratorSource? CreateEnumSource(Compilation compilation, TypeDeclarationSyntax type, EnumTypeInfo sourseInfo, EnumerationTypeInfo enumerationInfo)
    {
        var symbol = enumerationInfo.Symbol;
        var baseSymbol = symbol.BaseType;
        var generator = SyntaxGenerator.Clone(type);
        var bundleBuilder = new EnumBundleBuilder(compilation);
        var fields = SymbolReflection.GetEnumFields(sourseInfo.Symbol).ToArray();
        if (baseSymbol is null || baseSymbol.IsObject())
        {
            var enumerationName = symbol.Name;
            var enumerationNaming = CompositeProvider.DeclaredOnly(symbol);
            var baseNaming = CompositeProvider.DeclaredOnly(symbol.BaseType!);
            var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
            var enumService = new EnumService(bundleBuilder, enumerationType, fields);
            var providerType = SyntaxFactory.IdentifierName(enumerationName + "Provider");
            if (enumerationInfo.IsFlag)
            {
                if (CheckFlagEnumerationConstructor(compilation, enumerationNaming))
                {
                    //var providerBaseType = new GenericTypeNameInfo("FlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                    var emptyName = SymbolReflection.GetEnumField(fields, 0)?.Name ?? "Empty";
                    var provider = new FlagEnumerationProviderService(enumerationType, providerType, emptyName);
                    var displayType = new GenericTypeNameInfo("IFlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                    var naming = enumerationNaming.Inherit(baseNaming);
                    return new FlagEnumSource(generator, symbol, naming, enumService, provider, displayType, emptyName);
                }
            }
            else if (CheckEnumerationConstructor(compilation, enumerationNaming))
            {
                //var providerBaseType = new GenericTypeNameInfo("EnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var provider = new EnumerationProviderService(enumerationType, providerType);
                var displayType = new GenericTypeNameInfo("IEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var naming = enumerationNaming.Inherit(baseNaming);
                return new EnumSource(generator, symbol, naming, enumService, provider, displayType);
            }
        }
        else
        {
            var flagSymbol = EnumerationTypeInfo.GetFlagEnumerationBase(compilation);
            if (flagSymbol is null)
                return null;
            if (baseSymbol.IsBase(flagSymbol))
            {
                var enumerationName = symbol.Name;
                var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
                var providerName = enumerationName + "Provider";
                var providerType = SyntaxFactory.IdentifierName(providerName);
                var emptyName = SymbolReflection.GetEnumField(fields, 0)?.Name ?? "Empty";
                var provider = new FlagEnumerationProviderService(enumerationType, providerType, emptyName);
                var displayType = new GenericTypeNameInfo("IFlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var naming = CompositeProvider.Inherit(symbol);
                var enumService = new EnumService(bundleBuilder, enumerationType, fields);
                return new FlagEnumSource(generator, symbol, naming, enumService, provider, displayType, emptyName);
            }
            var enumerationSymbol = EnumerationTypeInfo.GetEnumerationBase(compilation);
            if (enumerationSymbol is null)
                return null;
            if (baseSymbol.IsBase(enumerationSymbol))
            {
                var enumerationName = symbol.Name;
                var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
                var providerName = enumerationName + "Provider";
                var providerType = SyntaxFactory.IdentifierName(providerName);
                var provider = new EnumerationProviderService(enumerationType, providerType);
                var displayType = new GenericTypeNameInfo("IEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var naming = CompositeProvider.Inherit(symbol);
                var enumService = new EnumService(bundleBuilder, enumerationType, fields);
                return new EnumSource(generator, symbol, naming, enumService, provider, displayType);
            }
        }
        return null;
    }
    /// <summary>
    /// 按枚举类型生成枚举类
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <param name="sourseInfo"></param>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    public IGeneratorSource? CreateComplexSource(Compilation compilation, TypeDeclarationSyntax type, EnumTypeInfo sourseInfo, ComplexTypeInfo typeInfo)
    {
        var sourseSymbol = sourseInfo.Symbol;
        var symbol = typeInfo.Symbol;
        var baseSymbol = symbol.BaseType;
        var generator = SyntaxGenerator.Clone(type);
        var bundleBuilder = new EnumBundleBuilder(compilation);
        var fields = SymbolReflection.GetEnumFields(sourseInfo.Symbol).ToArray();
        if (baseSymbol is null || baseSymbol.IsObject())
        {
            var enumerationNaming = CompositeProvider.DeclaredOnly(symbol);
            // 非枚举类不能有构造函数
            if (enumerationNaming.ConstructorCount() > 0)
                return null;
            var enumerationName = symbol.Name;
            var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
            var enumService = new EnumService(bundleBuilder, enumerationType, fields);
            var providerType = SyntaxFactory.IdentifierName(enumerationName + "Provider");
            if (sourseInfo.IsFlag)
            {
                var flagEnumerationSymbol = EnumerationTypeInfo.GetFlagEnumerationBase(compilation);
                if (flagEnumerationSymbol is null)
                    return null;
                var baseNaming = CompositeProvider.Create(flagEnumerationSymbol);
                var baseType = new TypeNameInfo("FlagEnumeration", EnumerationProviderService.Enumerations);
                generator.AddBaseType(baseType.Display(generator));
                //var providerBaseType = new GenericTypeNameInfo("FlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var emptyName = SymbolReflection.GetEnumField(fields, 0)?.Name ?? "Empty";
                var provider = new FlagEnumerationProviderService(enumerationType, providerType, emptyName);
                var displayType = new GenericTypeNameInfo("IFlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var naming = new InheritProvider(enumerationNaming.Typed(enumerationName), baseNaming);
                return new FlagEnumSource(generator, symbol, naming, enumService, provider, displayType, emptyName);
            }
            else
            {
                var enumerationSymbol = EnumerationTypeInfo.GetEnumerationBase(compilation);
                if (enumerationSymbol is null)
                    return null;
                var baseNaming = CompositeProvider.Create(enumerationSymbol);
                var baseType = new TypeNameInfo("Enumeration", EnumerationProviderService.Enumerations);
                generator.AddBaseType(baseType.Display(generator));
                //var providerBaseType = new GenericTypeNameInfo("EnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var provider = new EnumerationProviderService(enumerationType, providerType);
                var displayType = new GenericTypeNameInfo("IEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
                var naming = new InheritProvider(enumerationNaming.Typed(enumerationName), baseNaming);
                return new EnumSource(generator, symbol, naming, enumService, provider, displayType);
            }
        }
        else
        {
            var flagGenericInterface = EnumerationProviderBuilder.GetFlagGenericInterface(compilation);
            var flagSymbol = EnumerationTypeInfo.GetFlagEnumerationBase(compilation);
            if (flagGenericInterface is null || flagSymbol is null)
                return null;
            if (baseSymbol.IsInterface(flagGenericInterface.Construct(flagSymbol)))
            {
                var naming = CompositeProvider.Inherit(symbol);
                var @namespace = symbol.ContainingNamespace.ToName();
                var enumerationName = "FlagEnumeration";
                var providerName = symbol.Name;
                var enumService = new EnumService(bundleBuilder, SyntaxFactory.IdentifierName(enumerationName), [.. SymbolReflection.GetEnumFields(sourseSymbol)]);
                return new EnumProviderSource(generator, @namespace, providerName, enumerationName, enumService, FlagEnumerationProviderService.Create(enumerationName, providerName), naming);
            }
            var genericInterface = EnumerationProviderBuilder.GetGenericInterface(compilation);
            var enumerationSymbol = EnumerationTypeInfo.GetEnumerationBase(compilation);
            if (genericInterface is null || enumerationSymbol is null)
                return null;
            if (baseSymbol.IsInterface(genericInterface.Construct(enumerationSymbol)))
            {
                var naming = CompositeProvider.Inherit(symbol);
                var @namespace = symbol.ContainingNamespace.ToName();
                var enumerationName = "Enumeration";
                var providerName = symbol.Name;
                var enumService = new EnumService(bundleBuilder, SyntaxFactory.IdentifierName(enumerationName), [.. SymbolReflection.GetEnumFields(sourseSymbol)]);
                return new EnumProviderSource(generator, @namespace, providerName, enumerationName, enumService, EnumerationProviderService.Create(enumerationName, providerName), naming);
            }
        }
        return null;
    }
    /// <summary>
    /// 检查构造函数
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="naming"></param>
    /// <returns></returns>
    public static bool CheckEnumerationConstructor(Compilation compilation, CompositeProvider naming)
    {
        return naming.ConstructorCount() switch
        {
            0 => true,
            _ => CheckEnumerationConstructor(naming, compilation.GetStringSymbol(), compilation.GetLongSymbol()),
        };
    }
    /// <summary>
    /// 检查构造函数
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="naming"></param>
    /// <returns></returns>
    public static bool CheckFlagEnumerationConstructor(Compilation compilation, CompositeProvider naming)
    {
        var stringSymbol = compilation.GetStringSymbol();
        var flagsSymbol = compilation.GetISetSymbol(stringSymbol);
        if (flagsSymbol is null)
            return false;
        return naming.ConstructorCount() switch
        {
            0 => true,
            _ => naming.ConstructorContains(flagsSymbol, compilation.GetLongSymbol(), stringSymbol),
        };
    }
    /// <summary>
    /// 检查构造函数
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="enumerationSymbol"></param>
    /// <returns></returns>
    public static bool CheckFlagEnumerationConstructor(Compilation compilation, INamedTypeSymbol enumerationSymbol)
    {
        var stringSymbol = compilation.GetStringSymbol();
        var flagsSymbol = compilation.GetISetSymbol(stringSymbol);
        if (flagsSymbol is null)
            return false;
        return SymbolReflection.GetConstructors(enumerationSymbol)
            .Any(constructor => SymbolReflection.MatchParameterType(constructor.Parameters, flagsSymbol, compilation.GetLongSymbol(), stringSymbol));
    }
    /// <summary>
    /// 检查构造函数
    /// </summary>
    /// <param name="naming"></param>
    /// <param name="stringSymbol"></param>
    /// <param name="longSymbol"></param>
    /// <returns></returns>
    public static bool CheckEnumerationConstructor(CompositeProvider naming, INamedTypeSymbol stringSymbol, INamedTypeSymbol longSymbol)
        => naming.ConstructorContains(stringSymbol, longSymbol, stringSymbol);
    /// <summary>
    /// 检查基类
    /// </summary>
    /// <param name="baseSymbol"></param>
    /// <returns></returns>
    public static bool CheckBaseType(INamedTypeSymbol? baseSymbol)
    {
        if (baseSymbol is null)
            return true;
        return baseSymbol.IsObject();
    }
    ///// <summary>
    ///// 检查基类
    ///// </summary>
    ///// <param name="generator"></param>
    ///// <param name="typeSymbol"></param>
    ///// <param name="expected"></param>
    ///// <returns></returns>
    //public static INamedTypeSymbol? CheckBaseType(SyntaxGenerator generator, INamedTypeSymbol typeSymbol, INamedTypeSymbol expected)
    //{
    //    var baseSymbol = typeSymbol.BaseType;
    //    if (baseSymbol is null || baseSymbol.IsObject())
    //    {
    //        // 忽略有构造函数的类型
    //        if (typeSymbol.InstanceConstructors.Any(constructor => !constructor.IsImplicitlyDeclared))
    //            return null;
    //        generator.AddBaseType(generator.Display(expected));
    //        return expected;
    //    }
    //    if (baseSymbol.IsBase(expected))
    //        return baseSymbol;
    //    return null;
    //}
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly EnumerationTransform Instance = new();
}