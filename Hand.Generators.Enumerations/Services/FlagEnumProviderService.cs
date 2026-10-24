using Hand.Enums.Builders;
using Hand.Naming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enumerations.Services;

/// <summary>
/// 位标记枚举类型提供器服务
/// </summary>
/// <param name="bundleBuilder"></param>
/// <param name="enumerationType"></param>
/// <param name="fields"></param>
public class FlagEnumProviderService(EnumBundleBuilder bundleBuilder, TypeSyntax enumerationType, IFieldSymbol[] fields)
    : EnumService(bundleBuilder, enumerationType, fields)
{
    /// <summary>
    /// 逻辑空成员名
    /// </summary>
    public const string EmptyName = "Empty";

    /// <inheritdoc />
    public override void GenerateField(SyntaxGenerator generator, string name, long original, string description, bool isNew)
    {
        if(isNew && name == EmptyName)
        {
            var empty = _enumerationType.Property(name, CreateEnumeration(name, original, description))
                .Public()
                .Override();
            generator.AddProperty(empty);
        }
        else
        {
            base.GenerateField(generator, name, original, description, isNew);
        }
    }
    /// <inheritdoc />
    public override bool GenerateProperty(SyntaxGenerator generator, InheritProvider naming, string propertyName, string fieldName)
    {
        if (propertyName == EmptyName)
            return false;
        return base.GenerateProperty(generator, naming, propertyName, fieldName);
    }
}
