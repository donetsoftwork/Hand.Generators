using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hand;

/// <summary>
/// 语法树执行器
/// </summary>
public partial class SyntaxTreeScript(CSharpParseOptions options, string path, List<UsingDirectiveSyntax> usings, List<MetadataReference> references)
{
    #region 配置
    private readonly CSharpParseOptions _options = options;
    private readonly string _path = path;
    private readonly List<UsingDirectiveSyntax> _usings = usings;
    private readonly List<MetadataReference> _references = references;
    /// <summary>
    /// 配置
    /// </summary>
    public CSharpParseOptions Options
        => _options;
    /// <summary>
    /// 路径
    /// </summary>
    public string Path
        => _path;
    /// <summary>
    /// using
    /// </summary>
    public IReadOnlyCollection<UsingDirectiveSyntax> Usings
        => _usings;
    /// <summary>
    /// 引用
    /// </summary>
    public IReadOnlyCollection<MetadataReference> References
        => _references;
    #endregion
    #region Using
    /// <summary>
    /// 添加using
    /// </summary>
    /// <param name="usings"></param>
    public SyntaxTreeScript Using(params IEnumerable<UsingDirectiveSyntax> usings)
    {
        _usings.AddRange(usings);
        return this;
    }
    /// <summary>
    /// 添加using
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public SyntaxTreeScript Using(params IEnumerable<NameSyntax> names)
        => Using(names.Select(SyntaxFactory.UsingDirective));
    /// <summary>
    /// 添加using
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public SyntaxTreeScript Using(params IEnumerable<string> names)
        => Using(names.Select(SyntaxFactory.IdentifierName));
    #endregion
    #region Reference
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public SyntaxTreeScript Reference(MetadataReference reference)
    {
        _references.Add(reference);
        return this;
    }
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public SyntaxTreeScript Reference(Assembly assembly)
    {
        if (assembly.IsDynamic)
            return this;
        _references.Add(MetadataReference.CreateFromFile(assembly.Location));
        return this;
    }
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public SyntaxTreeScript Reference<T>()
        => Reference(typeof(T).Assembly);
    #endregion
    #region Create
    /// <summary>
    /// 构造执行器
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SyntaxTreeScript Create(string path)
        => new(new CSharpParseOptions(LanguageVersion.Latest), path, [], []);
    /// <summary>
    /// 构造执行器
    /// </summary>
    /// <returns></returns>
    public static SyntaxTreeScript Create()
        => new(new CSharpParseOptions(LanguageVersion.Latest), Environment.CurrentDirectory, [], []);
    /// <summary>
    /// 构造默认执行器
    /// </summary>
    /// <returns></returns>
    public static SyntaxTreeScript CreateDefault()
        => new(new CSharpParseOptions(LanguageVersion.Latest), Environment.CurrentDirectory, [_usingSystem], [.. Inner.Instance.References]);
    #endregion
    /// <summary>
    /// 默认实例
    /// </summary>
    public static SyntaxTreeScript Default
        => Inner.Instance;
    /// <summary>
    /// using System
    /// </summary>
    private static readonly UsingDirectiveSyntax _usingSystem = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System"));
    /// <summary>
    /// 获取引用
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<MetadataReference> GetReferences(Assembly[] assemblies)
    {
        return assemblies.Where(assembly => !assembly.IsDynamic)
            .Select(assembly => assembly.Location)
            .Select(location => MetadataReference.CreateFromFile(location));
    }
    /// <summary>
    /// 内部缓存
    /// </summary>
    internal static class Inner
    {
        internal static SyntaxTreeScript Instance = new(new CSharpParseOptions(LanguageVersion.Latest), Environment.CurrentDirectory, [_usingSystem], [.. GetReferences(AppDomain.CurrentDomain.GetAssemblies())]);
    }
}
