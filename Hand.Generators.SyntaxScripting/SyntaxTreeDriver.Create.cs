using Hand.Collections;
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
public partial class SyntaxTreeDriver(CSharpParseOptions options, string path, HashSet<UsingDirectiveSyntax> usings, List<MetadataReference> references)
{
    #region 配置
    private CSharpParseOptions _options = options;
    private readonly string _path = path;
    private readonly HashSet<UsingDirectiveSyntax> _usings = usings;
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
    /// <summary>
    /// 解析文档模式
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public SyntaxTreeDriver WithDocumentationComments(DocumentationMode mode = DocumentationMode.Parse)
    {
        _options = _options.WithDocumentationMode(mode);
        return this;
    }
    #region Using
    /// <summary>
    /// 添加using
    /// </summary>
    /// <param name="using"></param>
    public SyntaxTreeDriver Using(UsingDirectiveSyntax @using)
    {
        _usings.Add(@using);
        return this;
    }
    /// <summary>
    /// 添加using
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SyntaxTreeDriver Using(NameSyntax name)
        => Using(SyntaxFactory.UsingDirective(name));
    /// <summary>
    /// 添加using
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SyntaxTreeDriver Using(string name)
        => Using(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(name)));
    #endregion
    #region Reference
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public SyntaxTreeDriver Reference(MetadataReference reference)
    {
        _references.Add(reference);
        return this;
    }
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="withDocument"></param>
    /// <returns></returns>
    public SyntaxTreeDriver Reference(Assembly assembly, bool withDocument = false)
    {
        _references.AddRange(assembly.ToReferences(withDocument));
        return this;
    }
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="withDocument"></param>
    /// <returns></returns>
    public SyntaxTreeDriver Reference<T>(bool withDocument = false)
        => Reference(typeof(T).Assembly, withDocument);
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <param name="type"></param>
    /// <param name="withDocument"></param>
    /// <returns></returns>
    public SyntaxTreeDriver Reference(Type type, bool withDocument = false)
        => Reference(type.Assembly, withDocument);
    #endregion
    #region Create
    /// <summary>
    /// 构造执行器
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static SyntaxTreeDriver CreateDriver(string path)
        => new(new CSharpParseOptions(LanguageVersion.Latest), path, new(UsingDirectiveComparer.Instance), []);
    /// <summary>
    /// 构造执行器
    /// </summary>
    /// <returns></returns>
    public static SyntaxTreeDriver CreateDriver()
        => new(new CSharpParseOptions(LanguageVersion.Latest), Environment.CurrentDirectory, new(UsingDirectiveComparer.Instance), []);
    /// <summary>
    /// 构造默认执行器
    /// </summary>
    /// <returns></returns>
    public static SyntaxTreeDriver CreateDefaultDriver()
        => new(new CSharpParseOptions(LanguageVersion.Latest), Environment.CurrentDirectory, new([SyntaxGenerator.SystemDirective], UsingDirectiveComparer.Instance), [.. DefaultInner.Instance.References]);
    /// <summary>
    /// 构造默认执行器
    /// </summary>
    /// <returns></returns>
    public static SyntaxTreeDriver CreateScriptDriver()
        => new(new CSharpParseOptions(LanguageVersion.Latest, kind: SourceCodeKind.Script), Environment.CurrentDirectory, new([SyntaxGenerator.SystemDirective], UsingDirectiveComparer.Instance), [.. ScriptInner.Instance.References]);
    #endregion
    /// <summary>
    /// 默认实例
    /// </summary>
    public static SyntaxTreeDriver DefaultDriver
        => DefaultInner.Instance;
    /// <summary>
    /// 脚本实例
    /// </summary>
    public static SyntaxTreeDriver ScriptDriver
         => ScriptInner.Instance;
    /// <summary>
    /// 内部缓存
    /// </summary>
    internal static class DefaultInner
    {
        internal static SyntaxTreeDriver Instance = new(new CSharpParseOptions(LanguageVersion.Latest), Environment.CurrentDirectory, new([SyntaxGenerator.SystemDirective], UsingDirectiveComparer.Instance), [.. AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.ToReferences())]);
    }
    /// <summary>
    /// 内部缓存
    /// </summary>
    internal static class ScriptInner
    {
        internal static SyntaxTreeDriver Instance = new(new CSharpParseOptions(LanguageVersion.Latest, kind: SourceCodeKind.Script), Environment.CurrentDirectory, new([SyntaxGenerator.SystemDirective], UsingDirectiveComparer.Instance), [.. AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.ToReferences())]);
    }
}
