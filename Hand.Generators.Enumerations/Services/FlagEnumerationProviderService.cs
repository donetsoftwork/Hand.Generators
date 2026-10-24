using Hand.Naming;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enumerations.Services;

/// <summary>
/// 标记枚举类提供器服务
/// </summary>
/// <param name="enumerationType"></param>
/// <param name="providerType"></param>
/// <param name="emptyMemberName"></param>
public class FlagEnumerationProviderService(IdentifierNameSyntax enumerationType, IdentifierNameSyntax providerType, string emptyMemberName)
    : EnumerationProviderService(enumerationType, providerType)
{
    private readonly string _emptyMemberName = emptyMemberName;
    private readonly GenericTypeNameInfo _flagsType = GenericTypeNameInfo.Collection("ISet", SyntaxGenerator.StringType);

    /// <inheritdoc />
    public override void GenerateMembers(SyntaxGenerator generator, InheritProvider naming)
    {
        if(naming.TryDeclareField("Empty", out _))
        {
            var emptyProperty = _enumerationType.Property("Empty", _enumerationType.Access(_emptyMemberName))
                .Public()
                .Override()
                .WithSummary("逻辑空");
            generator.AddProperty(emptyProperty);
        }
        var flags = SyntaxFactory.IdentifierName("flags");
        var original = SyntaxFactory.IdentifierName("original");
        var description = SyntaxFactory.IdentifierName("description");
        var flagsType = _flagsType.Display(generator);
        var createFlagMethod = _enumerationType.Method("CreateFlag",
                flagsType.Parameter(flags.Identifier),
                SyntaxGenerator.LongType.Parameter(original.Identifier),
                SyntaxGenerator.StringType.Parameter(description.Identifier))
            .Protected()
            .Override()
            .WithExpressionBody(SyntaxGenerator.ExpressionBody(SyntaxGenerator.New([flags, original, description])))
            .WithSemicolonToken()
            .WithInheritdoc();

        //yield return GenerateConstructor(generator);
        generator.AddMethod(createFlagMethod);
        base.GenerateMembers(generator, naming);
    }
    ///// <summary>
    ///// 获取空成员(选取第一个)
    ///// </summary>
    ///// <param name="enumerationSymbol"></param>
    ///// <returns></returns>
    //public static string? GetEmptyMember(INamedTypeSymbol enumerationSymbol)
    //    => GetMembers(enumerationSymbol).FirstOrDefault();

    /// <summary>
    /// 创建枚举提供器服务
    /// </summary>
    /// <param name="enumerationName"></param>
    /// <param name="emptyMemberName"></param>
    /// <returns></returns>
    public new static FlagEnumerationProviderService Create(string enumerationName, string emptyMemberName)
        => Create(enumerationName, enumerationName + "Provider", emptyMemberName);
    /// <summary>
    /// 创建枚举提供器服务
    /// </summary>
    /// <param name="enumerationName"></param>
    /// <param name="providerName"></param>
    /// <param name="emptyMemberName"></param>
    /// <returns></returns>
    public static FlagEnumerationProviderService Create(string enumerationName, string providerName, string emptyMemberName)
    {
        var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
        var providerType = SyntaxFactory.IdentifierName(providerName);
        //var baseType = new GenericTypeNameInfo("FlagEnumerationProvider", Enumerations, new SyntaxWrapper<TypeSyntax>(enumerationType));
        return new FlagEnumerationProviderService(enumerationType, providerType, emptyMemberName);
    }
    ///// <summary>
    ///// 创建枚举提供器服务
    ///// </summary>
    ///// <param name="enumerationSymbol"></param>
    ///// <returns></returns>
    //public new static FlagEnumerationProviderService? Create(INamedTypeSymbol enumerationSymbol)
    //{
    //    var members = GetMembers(enumerationSymbol).ToArray();
    //    if (members.Length == 0)
    //        return null;
    //    var enumerationName = enumerationSymbol.Name;
    //    var enumerationType = SyntaxFactory.IdentifierName(enumerationName);
    //    var providerType = SyntaxFactory.IdentifierName(enumerationName + "Provider");
    //    var baseType = new GenericTypeNameInfo("FlagEnumerationProvider", "Hand.Enumerations", new SyntaxWrapper<TypeSyntax>(enumerationType));
    //    return new FlagEnumerationProviderService(enumerationType, providerType, baseType, members, members[0]);
    //}
}
