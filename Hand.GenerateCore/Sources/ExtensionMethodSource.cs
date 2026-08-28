using Hand.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Sources;

/// <summary>
/// 扩展方法数据源
/// </summary>
/// <param name="original"></param>
/// <param name="info"></param>
public class ExtensionMethodSource(MethodSource original, TypeNameInfo info)
    : IGeneratorSource
{
    #region 配置
    /// <summary>
    /// 原始数据源
    /// </summary>
    private readonly MethodSource _original = original;
    /// <summary>
    /// 扩展类型信息
    /// </summary>
    private readonly TypeNameInfo _info = info;
    /// <summary>
    /// 扩展方法参数
    /// </summary>
    public static readonly IdentifierNameSyntax ExtensionThis = SyntaxFactory.IdentifierName("@this");

    /// <summary>
    /// 扩展类信息
    /// </summary>
    public TypeNameInfo Info
        => _info;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_info.FullName}.{_original.MethodName}.g.cs";
    #endregion
    /// <summary>
    /// 创建方法声明
    /// </summary>
    /// <returns></returns>
    public MethodDeclarationSyntax CreateMethod(SyntaxGenerator generator)
    {
        var thisParameter = _original.ThisType.Parameter(ExtensionThis.Identifier)
            .This();
        var returnType = generator.Display(_original.ReturnInfo);
        var method = returnType.Method(_original.MethodName, [thisParameter, .. _original.CreateParameters()])
            .Public()
            .Static();
        return _original.BuildBody(generator, method, ExtensionThis);
    }
    /// <summary>
    /// 构建生成器
    /// </summary>
    /// <returns></returns>
    public SyntaxGenerator Generate()
    {
        var generator = CreateGenerator(_info);
        var method = CreateMethod(generator);
        generator.AddMethod(method);
        return generator;
    }
    /// <summary>
    /// 创建生成器
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static SyntaxGenerator CreateGenerator(TypeNameInfo info)
    {
        var type = SyntaxFactory.ClassDeclaration(info.TypeName);
        if (info.IsInternal)
            type = type.Internal();
        else
            type = type.Public();
        return SyntaxGenerator.Create(info.Namespace, type.Static().Partial());
    }
}
