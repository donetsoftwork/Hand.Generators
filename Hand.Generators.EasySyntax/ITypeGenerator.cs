using Hand.Scopes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand;

/// <summary>
/// 类语法树构造器
/// </summary>
public interface ITypeGenerator : ISyntaxScope
{
    #region Member
    /// <summary>
    /// 添加构造函数
    /// </summary>
    /// <param name="constructor"></param>
    void AddConstructor(ConstructorDeclarationSyntax constructor);
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="field"></param>
    void AddField(FieldDeclarationSyntax field);
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="property"></param>
    void AddProperty(PropertyDeclarationSyntax property);
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    void AddMethod(MethodDeclarationSyntax method);
    /// <summary>
    /// 增加其他成员
    /// </summary>
    /// <param name="members"></param>
    void AddOthers(params MemberDeclarationSyntax[] members);
    #endregion
    #region Type
    /// <summary>
    /// 应用类型修改
    /// </summary>
    /// <param name="modify"></param>
    void Apply(Func<TypeDeclarationSyntax, TypeDeclarationSyntax> modify);
    /// <summary>
    /// 增加基类
    /// </summary>
    /// <param name="baseType"></param>
    void AddBaseType(BaseTypeSyntax baseType);
    /// <summary>
    /// 增加主构造函数参数
    /// </summary>
    /// <param name="parameter"></param>
    void AddListParameter(ParameterSyntax parameter);
    #endregion
    /// <summary>
    /// 构造类型
    /// </summary>
    /// <returns></returns>
    TypeDeclarationSyntax BuildType();
}
