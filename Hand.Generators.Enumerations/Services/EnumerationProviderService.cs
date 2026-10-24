using Hand.Builders;
using Hand.Naming;
using Hand.Scopes;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enumerations.Services;

/// <summary>
/// 枚举类提供器服务
/// </summary>
/// <param name="enumerationType"></param>
/// <param name="providerType"></param>
public class EnumerationProviderService(IdentifierNameSyntax enumerationType, IdentifierNameSyntax providerType)
{
    #region 配置
    //private readonly ISyntaxDisplay<TypeSyntax> _baseType = baseType;

    //private readonly GenericTypeNameInfo _dictionaryType = GenericTypeNameInfo.Dictionary("Dictionary", SyntaxGenerator.StringType, enumerationType);
    private readonly IdentifierNameSyntax _providerType = providerType;

    /// <inheritdoc cref="EnumerationType" path="/summary" />
    protected readonly IdentifierNameSyntax _enumerationType = enumerationType;
    /// <summary>
    /// Hand.Enumerations
    /// </summary>
    public static readonly QualifiedNameSyntax Enumerations = SyntaxFactory.IdentifierName("Enumerations").Qualify(NamespaceScope.Hand);

    /// <summary>
    /// 枚举类
    /// </summary>
    public IdentifierNameSyntax EnumerationType
        => _enumerationType;
    /// <summary>
    /// 枚举提供器类
    /// </summary>
    public IdentifierNameSyntax ProviderType
        => _providerType;
    #endregion
    
    /// <summary>
    /// 生成实例字段
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isNew"></param>
    public FieldDeclarationSyntax GenerateInstance(string name = "Instance", bool isNew = false)
    {
        return _providerType.Field(name, SyntaxGenerator.New())
            .Public()
            .New(isNew)
            .Static()
            .ReadOnly();
    }
    /// <summary>
    /// 生成实例字段
    /// </summary>
    /// <param name="displayType"></param>
    /// <param name="name"></param>
    /// <param name="isNew"></param>
    public FieldDeclarationSyntax GenerateInstance(TypeSyntax displayType, string name = "Instance", bool isNew = false)
    {
        return displayType.Field(name, _providerType.New())
            .Public()
            .New(isNew)
            .Static()
            .ReadOnly();
    }
    /// <summary>
    /// 生成成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="naming"></param>
    public virtual void GenerateMembers(SyntaxGenerator generator, InheritProvider naming)
    {
        //generator.AddBaseType(_baseType.Display(generator));
    }
    ///// <summary>
    ///// 生成构造函数
    ///// </summary>
    ///// <param name="generator"></param>
    //public virtual ConstructorDeclarationSyntax GenerateConstructor(SyntaxGenerator generator)
    //{
    //    var dictionary = CreateDictionary(generator, _dictionaryType, _members);
    //    return generator.Type.Constructor()
    //        .WithBaseInitializer(dictionary)
    //        .WithBody(SyntaxFactory.Block())
    //        .Internal();
    //    //generator.AddConstructor(constructor);
    //    //return constructor;
    //}
    ///// <summary>
    ///// 构造枚举字典
    ///// </summary>
    ///// <param name="generator"></param>
    ///// <param name="dictionaryType"></param>
    ///// <param name="members"></param>
    ///// <returns></returns>
    //public ObjectCreationExpressionSyntax CreateDictionary(SyntaxGenerator generator, GenericTypeNameInfo dictionaryType, string[] members)
    //{
    //    var builder = new DictionaryBuilder();
    //    foreach (var member in members)
    //        builder.Add(SyntaxGenerator.Literal(member), _enumerationType.Access(member));
    //    return builder.Build(dictionaryType.Display(generator));
    //}
    /// <summary>
    /// 生成构造函数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="enumerationType"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static ConstructorDeclarationSyntax GenerateConstructor(SyntaxGenerator generator, TypeSyntax enumerationType, string[] members)
    {
        var dictionaryType = GenericTypeNameInfo.Dictionary("Dictionary", SyntaxGenerator.StringType, enumerationType);
        var builder = new DictionaryBuilder();
        foreach (var member in members)
            builder.Add(SyntaxGenerator.Literal(member), enumerationType.Access(member));
        var dictionary = builder.Build(dictionaryType.Display(generator));
        return generator.Declaration.Constructor()
            .WithBaseInitializer(dictionary)
            .WithBody(SyntaxFactory.Block())
            .Private()
            .WithSummary("构造函数");
    }
    /// <summary>
    /// 创建枚举提供器服务
    /// </summary>
    /// <param name="enumerationName">枚举类名</param>
    /// <param name="providerName">提供器类名</param>
    /// <returns></returns>
    public static EnumerationProviderService Create(string enumerationName, string providerName)
    {
        var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
        var providerType = SyntaxFactory.IdentifierName(providerName);
        //var baseType = new GenericTypeNameInfo("EnumerationProvider", Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
        return new EnumerationProviderService(enumerationType, providerType);
    }
    /// <summary>
    /// 创建枚举提供器服务
    /// </summary>
    /// <param name="enumerationName">枚举类名</param>
    /// <returns></returns>
    public static EnumerationProviderService Create(string enumerationName)
        => Create(enumerationName, enumerationName + "Provider");
}
