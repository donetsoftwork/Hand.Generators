using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Sources;

/// <summary>
/// 方法数据源
/// </summary>
/// <param name="compilation"></param>
/// <param name="methodName"></param>
/// <param name="thisType"></param>
/// <param name="returnInfo"></param>
public abstract class MethodSource(Compilation compilation, string methodName, TypeSyntax thisType, ITypeSymbolInfo returnInfo)
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
    protected readonly ITypeSymbolInfo _returnInfo = returnInfo;
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
    /// 返回类型信息
    /// </summary>
    public ITypeSymbolInfo ReturnInfo
        => _returnInfo;
    #endregion

    /// <summary>
    /// 创建参数列表
    /// </summary>
    /// <returns></returns>
    public virtual SeparatedSyntaxList<ParameterSyntax> CreateParameters()
        => [];
    /// <summary>
    /// 创建方法声明
    /// </summary>
    ///  <param name="generator"></param>
    /// <returns></returns>
    public virtual MethodDeclarationSyntax CreateMethod(SyntaxGenerator generator)
    {
        var returnType = generator.Display(_returnInfo);
        var method = returnType.Method(_methodName, CreateParameters())
            .Public();
        return BuildBody(generator, method, SyntaxGenerator.ThisExpression);
    }
    /// <summary>
    /// 构建方法体
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="method"></param>
    /// <param name="this"></param>
    /// <returns></returns>
    public abstract MethodDeclarationSyntax BuildBody(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this);
}
