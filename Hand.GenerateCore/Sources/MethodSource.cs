using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Sources;

/// <summary>
/// 方法数据源
/// </summary>
/// <param name="compilation"></param>
/// <param name="methodName"></param>
/// <param name="thisType"></param>
/// <param name="returnType"></param>
public abstract class MethodSource(Compilation compilation, string methodName, TypeSyntax thisType, TypeSyntax returnType)
{
    #region 配置
    /// <summary>
    /// 编译对象
    /// </summary>
    protected readonly Compilation _compilation = compilation;
    /// <summary>
    /// 方法名
    /// </summary>
    protected readonly string _methodName = methodName;
    /// <summary>
    /// 当前类型
    /// </summary>
    protected readonly TypeSyntax _thisType = thisType;
    /// <summary>
    /// 返回类型
    /// </summary>
    protected readonly TypeSyntax _returnType = returnType;
    /// <summary>
    /// 编译对象
    /// </summary>
    public Compilation Compilation
        => _compilation;
    /// <summary>
    /// 方法名
    /// </summary>
    public string MethodName
        => _methodName;
    /// <summary>
    /// 当前类型
    /// </summary>
    public TypeSyntax ThisType 
        => _thisType;
    /// <summary>
    /// 返回类型
    /// </summary>
    public TypeSyntax ReturnType 
        => _returnType;
    #endregion

    /// <summary>
    /// 创建参数列表
    /// </summary>
    /// <returns></returns>
    public virtual ParameterSyntax[] CreateParameters()
        => [];
    /// <summary>
    /// 创建方法声明
    /// </summary>
    /// <returns></returns>
    public virtual MethodDeclarationSyntax CreateMethod()
    {
        var method = _returnType.Method(_methodName, CreateParameters())
            .Public();
        return BuildBody(method, SyntaxGenerator.ThisExpression);
    }
    ///// <summary>
    ///// 创建方法声明
    ///// </summary>
    ///// <returns></returns>
    //public MethodDeclarationSyntax CreateMethod(bool isExtension, ParameterSyntax[] parameters, TypeSyntax returnType)
    //{
    //    var method = returnType.Method(_methodName, parameters);
    //    ExpressionSyntax @this;
    //    if (isExtension)
    //    {
    //        // 定义internal的静态扩展方法
    //        method = method.Internal().Static();
    //        @this = ExtensionThis;
    //    }
    //    else
    //    {
    //        method = method.Public();
    //        @this = SyntaxGenerator.ThisExpression;
    //    }
    //    return BuildBody(method, @this);
    //}
    /// <summary>
    /// 构建方法体
    /// </summary>
    /// <param name="method"></param>
    /// <param name="this"></param>
    /// <returns></returns>
    public abstract MethodDeclarationSyntax BuildBody(MethodDeclarationSyntax method, ExpressionSyntax @this);
}
