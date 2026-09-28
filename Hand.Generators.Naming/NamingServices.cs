using Hand.Members;
using Hand.Naming;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 命名扩展方法
/// </summary>
public static class NamingServices
{
    /// <summary>
    /// 类成员命名器(CS0542,成员不能与类同名)
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypedProvider Typed(this INamingProvider provider, string typeName)
        => new(typeName, provider);
    #region Composite
    /// <summary>
    /// 组合命名
    /// </summary>
    /// <param name="kinds"></param>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static CompositeProvider Composite(this KindContainer kinds, MethodContainer methods)
        => new(methods, kinds);
    /// <summary>
    /// 组合命名
    /// </summary>
    /// <param name="kinds"></param>
    /// <returns></returns>
    public static CompositeProvider Composite(this KindContainer kinds)
        => new(new MethodContainer(), kinds);
    /// <summary>
    /// 组合命名
    /// </summary>
    /// <param name="methods"></param>
    /// <param name="kinds"></param>
    /// <returns></returns>
    public static CompositeProvider Composite(this MethodContainer methods, KindContainer kinds)
        => new(methods, kinds);
    /// <summary>
    /// 组合命名
    /// </summary>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static CompositeProvider Composite(this MethodContainer methods)
        => new(methods, new KindContainer());
    /// <summary>
    /// 组合命名
    /// </summary>
    /// <param name="kinds"></param>
    /// <param name="methods"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static CompositeProvider Composite(this KindContainer kinds, MethodContainer methods, string typeName)
        => new(methods, new TypedProvider(typeName, kinds));
    /// <summary>
    /// 组合命名
    /// </summary>
    /// <param name="methods"></param>
    /// <param name="kinds"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static CompositeProvider Composite(this MethodContainer methods, KindContainer kinds, string typeName)
        => new(methods, new TypedProvider(typeName, kinds));
    #endregion
    /// <summary>
    /// 继承命名
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="base"></param>
    /// <returns></returns>
    public static InheritProvider Inherit(this INamingProvider provider, INamingProvider @base)
        => new(provider, @base);
    #region AddKind
    /// <summary>
    /// 添加成员类型
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="identifier"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddKind(this INamingProvider provider, SyntaxToken identifier, MemberKind kind)
        => provider.AddKind(identifier.ValueText, kind);
    /// <summary>
    /// 添加成员类型
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="kind"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddKind(this INamingProvider provider, MemberKind kind, IEnumerable<string> names)
    {
        foreach (var name in names)
            provider.AddKind(name, kind);
    }
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddField(this INamingProvider provider, string name)
        => provider.AddKind(name, MemberKind.Field);
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddProperty(this INamingProvider provider, string name)
        => provider.AddKind(name, MemberKind.Property);
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddField(this INamingProvider provider, FieldDeclarationSyntax field)
        => provider.AddKind(field.Declaration.Variables.First().Identifier.ValueText, MemberKind.FieldDeclaration);
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public static void AddProperty(this INamingProvider provider, PropertyDeclarationSyntax property)
    {
        // 忽略显示实现接口的属性
        if (property.ExplicitInterfaceSpecifier is null)
            provider.AddKind(property.Identifier.ValueText, MemberKind.PropertyDeclaration);
    }
    /// <summary>
    /// 尝试定义字段
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool TryDeclareField(this INamingProvider provider, string name)
    {
        if (provider.Contains(name))
            return false;
        provider.AddKind(name, MemberKind.FieldDeclaration);
        return true;
    }
    /// <summary>
    /// 尝试定义属性
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool TryDeclareProperty(this INamingProvider provider, string name)
    {
        if (provider.Contains(name))
            return false;
        provider.AddKind(name, MemberKind.PropertyDeclaration);
        return true;
    }
    /// <summary>
    /// 尝试定义构造函数
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="parameterSymbols"></param>
    /// <returns></returns>
    public static bool TryDeclareConstructor(this INamingProvider provider, params ITypeSymbol[] parameterSymbols)
        => provider.TryDeclareMethod(MethodContainer.ConstructorName, parameterSymbols);
    /// <summary>
    /// 检查重名(如果重名尝试后缀)
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <param name="tryTimes"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static bool CheckName(this INamingProvider provider, ref string name, int tryTimes = 10, string suffix = "_")
    {
        var checkName = name;
        for (int i = 1; i <= tryTimes; i++)
        {
            if (provider.Contains(checkName))
            {
                checkName = name + suffix + i;
            }
            else
            {
                name = checkName;
                return true;
            }
            
        }
        return false;
    }
    #endregion
}
