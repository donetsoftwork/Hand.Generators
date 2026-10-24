using Hand.Documentation;
using Hand.Enums.Builders;
using Hand.Naming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Enumerations.Services;

/// <summary>
/// 枚举类型服务
/// </summary>
/// <param name="bundleBuilder"></param>
/// <param name="enumerationType"></param>
/// <param name="fields"></param>
public class EnumService(EnumBundleBuilder bundleBuilder, TypeSyntax enumerationType, IFieldSymbol[] fields)
{
    #region 配置
    private readonly EnumBundleBuilder _bundleBuilder = bundleBuilder;
    /// <summary>
    /// 枚举类
    /// </summary>
    protected readonly TypeSyntax _enumerationType = enumerationType;
    private readonly IFieldSymbol[] _fields = fields;

    /// <summary>
    /// 枚举类
    /// </summary>
    public TypeSyntax EnumerationType 
        => _enumerationType;
    /// <summary>
    /// 枚举字段
    /// </summary>
    public IFieldSymbol[] Fields 
        => _fields;
    #endregion
    /// <summary>
    /// 生成成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="naming"></param>
    public virtual (string[], Dictionary<long, string>, Dictionary<string, string>) GenerateMembers(SyntaxGenerator generator, InheritProvider naming)
    {
        var originalNames = new Dictionary<long, string>();
        var aliasNames = new Dictionary<string, string>();
        var list = new List<string>(_fields.Length);
        foreach (var item in _fields)
        {
            var name = item.Name;
            var value = System.Convert.ToInt64(item.ConstantValue);
            // 相同值作为别名处理
            if (originalNames.TryGetValue(value, out var existingName))
            {
                aliasNames[name] = existingName;
                CheckEnumMember(aliasNames, item, existingName);
            }
            else if (naming.TryDeclareField(name, out var isNew))
            {
                originalNames.Add(value, name);
                var summary = SummaryCacher.GetSummary(item);
                GenerateField(generator, name, value, summary, isNew);
                list.Add(name);
                CheckEnumMember(aliasNames, item, name);
            }
        }
        foreach (var item in aliasNames)
        {
            //var aliasName = item.Key;
            //if (GenerateProperty(generator, naming, aliasName, item.Value))
            //    list.Add(aliasName);
            GenerateProperty(generator, naming, item.Key, item.Value);
        }
        return (list.ToArray(), originalNames, aliasNames);
    }
    /// <summary>
    /// 生成字段
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <param name="original"></param>
    /// <param name="summary"></param>
    /// <param name="isNew"></param>
    /// <returns></returns>
    public virtual void GenerateField(SyntaxGenerator generator, string name, long original, string summary, bool isNew)
    {
        var field = _enumerationType.Field(name, CreateEnumeration(name, original, summary))
            .Public()
            .New(isNew)
            .Static()
            .ReadOnly()
            .WithSummary(summary);
        generator.AddField(field);
    }
    /// <summary>
    /// 生成别名属性
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="naming"></param>
    /// <param name="propertyName"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public virtual bool GenerateProperty(SyntaxGenerator generator, InheritProvider naming, string propertyName, string fieldName)
    {
        if (naming.TryDeclareProperty(propertyName, out var isNew))
        {
            var property = _enumerationType.Property(propertyName, SyntaxFactory.IdentifierName(fieldName))
                .Public()
                .New(isNew)
                .Static()
                .WithSummary(fieldName);
            generator.AddProperty(property);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 创建枚举成员
    /// </summary>
    /// <param name="name"></param>
    /// <param name="original"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static ExpressionSyntax CreateEnumeration(string name, long original, string description)
        => SyntaxGenerator.New([
            SyntaxGenerator.Literal(name),
            SyntaxGenerator.Literal(original),
            SyntaxGenerator.Literal(description)
        ]);
    /// <summary>
    /// 处理EnumMember标记
    /// </summary>
    /// <param name="aliasNames"></param>
    /// <param name="field"></param>
    /// <param name="existingName"></param>
    public void CheckEnumMember(Dictionary<string, string> aliasNames, IFieldSymbol field, string existingName)
    {
        var member = _bundleBuilder.GetEnumMemberName(field);
        // 已经存在不处理,避免覆盖原有的别名
        if (string.IsNullOrWhiteSpace(member) || aliasNames.ContainsKey(member))
            return;
        // EnumMember作为别名处理
        aliasNames.Add(member, existingName);
    }
}
