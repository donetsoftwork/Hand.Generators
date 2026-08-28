using Hand.Builders;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Sources;

/// <summary>
/// 类型转化生成源
/// </summary>
/// <param name="builder"></param>
/// <param name="generator"></param>
/// <param name="metadataName"></param>
/// <param name="methods"></param>
public class ConvertToSource(ConvertBuilder builder, SyntaxGenerator generator, string metadataName, ComplexSource[] methods)
    : IGeneratorSource
{
    /// <summary>
    /// 类型转化生成源
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="type"></param>
    /// <param name="metadataName"></param>
    /// <param name="methods"></param>
    public ConvertToSource(ConvertBuilder builder, TypeDeclarationSyntax type, string metadataName, ComplexSource[] methods)
        : this(builder, SyntaxGenerator.Clone(type), metadataName, methods)
    {
    }
    #region 配置
    private readonly ConvertBuilder _builder = builder;
    private readonly SyntaxGenerator _generator = generator;
    /// <summary>
    /// 类语法树构造器
    /// </summary>
    public SyntaxGenerator Generator
        => _generator;
    /// <summary>
    /// 类型符号
    /// </summary>
    private readonly string _metadataName = metadataName;
    private readonly ComplexSource[] _methods = methods;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_metadataName}.ConvertTo.g.cs";
    /// <summary>
    /// 转化构造器
    /// </summary>
    public ConvertBuilder Builder 
        => _builder;
    /// <summary>
    /// 方法
    /// </summary>
    public ComplexSource[] Methods 
        => _methods;
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        foreach (var method in _methods)
            _generator.AddMethod(method.CreateMethod(_generator));
        return _generator;
    }
}
