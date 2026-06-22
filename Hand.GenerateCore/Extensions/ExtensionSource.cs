using Hand.Builders;
using Hand.Members;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Extensions;

/// <summary>
/// 扩展方法数据源
/// </summary>
/// <param name="compilation"></param>
/// <param name="info"></param>
/// <param name="isInternal"></param>
/// <param name="methodName"></param>
/// <param name="thisType"></param>
/// <param name="returnType"></param>
public abstract class ExtensionSource(Compilation compilation, TypeNameInfo info, bool isInternal, string methodName, TypeSyntax thisType, TypeSyntax returnType)
    : IGeneratorSource
{
    #region 配置
    /// <summary>
    /// 编译对象
    /// </summary>
    protected readonly Compilation _compilation = compilation;
    /// <summary>
    /// 扩展类信息
    /// </summary>
    protected readonly TypeNameInfo _info = info;
    /// <summary>
    /// 扩展类符号
    /// </summary>
    protected readonly INamedTypeSymbol? _symbol = compilation.GetTypeByMetadataName(info.FullName);
    /// <summary>
    /// 是否为internal
    /// </summary>
    protected readonly bool _isInternal = isInternal;
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
    /// this参数名
    /// </summary>
    public static readonly IdentifierNameSyntax This = SyntaxFactory.IdentifierName("@this");
    /// <summary>
    /// 编译对象
    /// </summary>
    public Compilation Compilation
        => _compilation;
    /// <summary>
    /// 扩展类信息
    /// </summary>
    public TypeNameInfo Info
        => _info;
    /// <summary>
    /// 扩展类符号
    /// </summary>
    public INamedTypeSymbol? Symbol
        => _symbol;
    /// <summary>
    /// 是否为internal
    /// </summary>
    public bool IsInternal
        => _isInternal;
    /// <summary>
    /// 方法名
    /// </summary>
    public string MethodName
        => _methodName;
    /// <summary>
    /// 返回类型
    /// </summary>
    public TypeSyntax ReturnType 
        => _returnType;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_info.FullName}.{_methodName}.g.cs";
    #endregion

    /// <summary>
    /// 创建参数列表
    /// </summary>
    /// <param name="thisParameter"></param>
    /// <returns></returns>
    protected virtual ParameterSyntax[] CreateParameters(ParameterSyntax thisParameter)
        => [thisParameter];
    /// <summary>
    /// 创建方法声明
    /// </summary>
    /// <returns></returns>
    public MethodDeclarationSyntax CreateMethodDeclaration()
    {
        var thisParameter = _thisType.Parameter(This.Identifier)
            .This();
        var parameters = CreateParameters(thisParameter);
        var method = _returnType.Method(_methodName, parameters);
        if (_isInternal)
            method = method.Internal();
        else
            method = method.Public();
        return BuildBody(method.Static().ToBuilder(), This);
    }
    /// <summary>
    /// 构建生成器
    /// </summary>
    /// <returns></returns>
    public SyntaxGenerator Generate()
    {
        var generator = CreateGenerator(_info, _isInternal);
        var method = CreateMethodDeclaration();
        generator.AddMethod(method);
        return generator;
    }
    /// <summary>
    /// 构建方法体
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="this"></param>
    /// <returns></returns>
    protected abstract MethodDeclarationSyntax BuildBody(MethodBodyBuilder<MethodDeclarationSyntax> builder, ExpressionSyntax @this);
    /// <summary>
    /// 创建生成器
    /// </summary>
    /// <param name="info"></param>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public static SyntaxGenerator CreateGenerator(TypeNameInfo info, bool isInternal = true)
    {
        var type = SyntaxFactory.ClassDeclaration(info.TypeName);
        if (isInternal)
            type = type.Internal();
        else
            type = type.Public();
        return SyntaxGenerator.Create(info.Namespace, type.Static().Partial());
    }
}
