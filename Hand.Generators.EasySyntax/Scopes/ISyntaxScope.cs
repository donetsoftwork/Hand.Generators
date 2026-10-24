using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Scopes;

/// <summary>
/// 语法作用域
/// </summary>
public interface ISyntaxScope
{
    #region Using
    /// <summary>
    /// 命名
    /// </summary>
    NameSyntax Name { get; }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="directive"></param>
    void Using(UsingDirectiveSyntax directive);
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="namespace"></param>
    void Using(NameSyntax @namespace);
    #endregion
    /// <summary>
    /// 增加类型
    /// </summary>
    /// <param name="type"></param>
    void Add(ITypeGenerator type);
    ///// <summary>
    ///// 展示类名
    ///// 支持泛型定义不支持闭合泛型
    ///// 闭合泛型请使用Hand.GenerateCore的扩展方法TypeSyntax Display(this SyntaxGenerator generator, GenericTypeInfo info)
    ///// 暂不支持内部类
    ///// </summary>
    ///// <param name="symbol"></param>
    ///// <returns></returns>
    //TypeSyntax Display(INamedTypeSymbol symbol);
    /// <summary>
    /// 展示类名
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <returns></returns>
    NameSyntax Display(string typeName, NameSyntax @namespace);
    /// <summary>
    /// 展示成员(静态属性或字段)
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    NameSyntax DisplayMember(string typeName, NameSyntax @namespace, SimpleNameSyntax memberName);
    /// <summary>
    /// 构造语法根
    /// </summary>
    /// <returns></returns>
    CompilationUnitSyntax BuildUnit();
}
