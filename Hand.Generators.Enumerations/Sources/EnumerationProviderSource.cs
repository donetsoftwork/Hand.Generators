using Hand.Enumerations.Services;
using Hand.Members;
using Hand.Naming;
using Hand.Sources;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enumerations.Sources;

/// <summary>
/// 枚举类提供器源
/// </summary>
public class EnumerationProviderSource(SyntaxGenerator generator, NameSyntax @namespace, string providerName, EnumerationProviderService service, InheritProvider naming, string[] members)
    : GeneratorSourceBase
{
    #region 配置
    private readonly SyntaxGenerator _generator = generator;
    private readonly NameSyntax _namespace = @namespace;
    private readonly string _providerName = providerName;
    /// <summary>
    /// 实例名
    /// </summary>
    public const string InstanceName = "Instance";
    private readonly EnumerationProviderService _service = service;
    /// <summary>
    /// 命名
    /// </summary>
    protected readonly InheritProvider _naming = naming;
    private readonly string[] _members = members;

    /// <summary>
    /// 命名空间
    /// </summary>
    public NameSyntax Namespace
        => _namespace;
    /// <summary>
    /// 提供器类名
    /// </summary>
    public string ProviderName
        => _providerName;
    /// <summary>
    /// 成员名
    /// </summary>
    public string[] Members
        => _members;
    /// <inheritdoc />
    public override string GenerateFileName 
        => _namespace + "." + _providerName + ".g.cs";
    #endregion

    /// <summary>
    /// 获取实例成员
    /// </summary>
    /// <returns></returns>
    public StaticMember GetInstanceMember()
        => new(_namespace, _providerName, SyntaxFactory.IdentifierName(InstanceName));

    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        // 生成构造函数
        var constructor = EnumerationProviderService.GenerateConstructor(_generator, _service.EnumerationType, _members);
        if(_naming.TryDeclareField(InstanceName, out var isNew))
        {
            // 生成实例字段
            var instanceField = _service.GenerateInstance(InstanceName, isNew)
                .WithSummary("单例");
            _generator.AddOthers(instanceField);
        }
        // 添加成员
        _service.GenerateMembers(_generator, _naming);
        //.AddMembers(constructor, instanceField);
        _generator.AddConstructor(constructor);
        
        // 类型修改覆盖生成器内部类型
        //generator.Apply(type => declaration);
        return _generator;
    }
    /// <summary>
    /// 创建枚举类提供源
    /// </summary>
    /// <param name="enumerationSymbol"></param>
    /// <param name="naming"></param>
    /// <returns></returns>
    public static EnumerationProviderSource Create(INamedTypeSymbol enumerationSymbol, InheritProvider naming)
    {
        var enumerationName = enumerationSymbol.Name;
        var providerName = enumerationName + "Provider";
        var service = EnumerationProviderService.Create(enumerationName, providerName);
        var @namespace = enumerationSymbol.ContainingNamespace.ToName();
        var declaration = SyntaxFactory.ClassDeclaration(providerName)
            .Public();
        var generator = SyntaxGenerator.Create(@namespace, declaration);
        var providerBaseType = new GenericTypeNameInfo("EnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(SyntaxFactory.IdentifierName(enumerationName)));
        generator.AddBaseType(providerBaseType.Display(generator));
        return new(generator, @namespace, providerName, service, naming, [.. GetMembers(enumerationSymbol)]);
    }
    /// <summary>
    /// 创建枚举类提供源
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationSymbol"></param>
    /// <param name="naming"></param>
    /// <param name="enumerationName"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static EnumerationProviderSource Create(SyntaxGenerator generator, INamedTypeSymbol enumerationSymbol, InheritProvider naming, string enumerationName, string providerName)
    {
        //var enumerationName = enumerationSymbol.Name;
        //var providerName = enumerationName + "Provider";
        var service = EnumerationProviderService.Create(enumerationName, providerName);
        var @namespace = enumerationSymbol.ContainingNamespace.ToName();
        return new(generator, @namespace, providerName, service, naming, [.. GetMembers(enumerationSymbol)]);
    }
    ///// <summary>
    ///// 创建标记枚举类提供源
    ///// </summary>
    ///// <param name="enumerationSymbol"></param>
    ///// <param name="naming"></param>
    ///// <param name="emptyMemberName"></param>
    ///// <returns></returns>
    //public static EnumerationProviderSource CreateFlag(INamedTypeSymbol enumerationSymbol, InheritProvider naming, string emptyMemberName)
    //{
    //    var enumerationName = enumerationSymbol.Name;
    //    var providerName = enumerationName + "Provider";
    //    var service = FlagEnumerationProviderService.Create(enumerationName, providerName, emptyMemberName);
    //    var @namespace = enumerationSymbol.ContainingNamespace.ToName();
    //    var declaration = SyntaxFactory.ClassDeclaration(providerName)
    //        .Public();
    //    var generator = SyntaxGenerator.Create(@namespace, declaration);
    //    var providerBaseType = new GenericTypeNameInfo("FlagEnumerationProvider", EnumerationProviderService.Enumerations, new SyntaxWrapper<TypeSyntax>(SyntaxFactory.IdentifierName(enumerationName)));
    //    generator.AddBaseType(providerBaseType.Display(generator));
    //    return new(generator, @namespace, providerName, service, naming, [.. GetMembers(enumerationSymbol)]);
    //}
    /// <summary>
    /// 创建标记枚举类提供源
    /// </summary>
    /// <param name="enumerationSymbol"></param>
    /// <param name="naming"></param>
    /// <returns></returns>
    public static EnumerationProviderSource? CreateFlag(INamedTypeSymbol enumerationSymbol, InheritProvider naming)
    {
        var members = GetMembers(enumerationSymbol)
            .ToArray();
        if (members.Length == 0)
            return null;
        var enumerationName = enumerationSymbol.Name;
        var providerName = enumerationName + "Provider";
        var service = FlagEnumerationProviderService.Create(enumerationName, providerName, members[0]);
        var @namespace = enumerationSymbol.ContainingNamespace.ToName();
        var declaration = SyntaxFactory.ClassDeclaration(providerName)
            .Public();
        var generator = SyntaxGenerator.Create(@namespace, declaration);
        return new(generator, @namespace, providerName, service, naming, members);
    }
    /// <summary>
    /// 创建标记枚举类提供源
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationSymbol"></param>
    /// <param name="naming"></param>
    /// <param name="enumerationName"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static EnumerationProviderSource? CreateFlag(SyntaxGenerator generator, INamedTypeSymbol enumerationSymbol, InheritProvider naming, string enumerationName, string providerName)
    {
        var members = GetMembers(enumerationSymbol)
            .ToArray();
        if (members.Length == 0)
            return null;
        //var enumerationName = enumerationSymbol.Name;
        //var providerName = enumerationName + "Provider";
        var service = FlagEnumerationProviderService.Create(enumerationName, providerName, members[0]);
        var @namespace = enumerationSymbol.ContainingNamespace.ToName();
        return new(generator, @namespace, providerName, service, naming, members);
    }
    /// <summary>
    /// 获取枚举成员
    /// </summary>
    /// <param name="enumerationSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetMembers(INamedTypeSymbol enumerationSymbol)
    {
        foreach (var item in enumerationSymbol.GetMembers())
        {
            if (item.DeclaredAccessibility == Accessibility.Public && item.IsStatic)
            {
                if (item.Kind == SymbolKind.Field && item is IFieldSymbol field && enumerationSymbol.Equals(field.Type, SymbolEqualityComparer.Default))
                    yield return item.Name;
                else if (item.Kind == SymbolKind.Property && item is IPropertySymbol property && enumerationSymbol.Equals(property.Type, SymbolEqualityComparer.Default))
                    yield return item.Name;
            }
        }
    }
}