using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Hand;

/// <summary>
/// 脚本扩展方法
/// </summary>
public static class SyntaxScriptingServices
{
    /// <summary>
    /// 转化为脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="compilation"></param>
    /// <returns></returns>
    internal static SyntaxTreeScript<T> ToScript<T>(this CSharpCompilation compilation)
        => new(compilation);
    /// <summary>
    /// 执行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="compilation"></param>
    /// <param name="globals"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static Task<T> ExecuteAsync<T>(this CSharpCompilation compilation, object? globals = null, CancellationToken cancellation = default)
    {
        return new SyntaxTreeScript<T>(compilation)
            .ExecuteAsync(globals, cancellation);
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="globals"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static Task<object> ExecuteAsync(this CSharpCompilation compilation, object? globals = null, CancellationToken cancellation = default)
    {
        return new SyntaxTreeScript<object>(compilation)
            .ExecuteAsync(globals, cancellation);
    }
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <param name="withDocument"></param>
    /// <returns></returns>
    public static Compilation WithReference(this Compilation compilation, Type type, bool withDocument = false)
    {
        var fullName = type.FullName;
        if (fullName is null || compilation.GetTypeByMetadataName(fullName) is not null)
            return compilation;
        var references = type.Assembly.ToReferences(withDocument)
            .ToArray();
        if (references.Length > 0)
            return compilation.AddReferences(references);
        return compilation;
    }
    /// <summary>
    /// 转化为程序集引用
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="withDocument"></param>
    /// <returns></returns>
    public static IEnumerable<MetadataReference> ToReferences(this Assembly assembly, bool withDocument = false)
    {
        if (assembly.IsDynamic)
            yield break;
        var location = assembly.Location;
        if (string.IsNullOrEmpty(location))
            yield break;
        DocumentationProvider? documentation = withDocument ? XmlDocumentationProvider.CreateFromFile(Path.ChangeExtension(location, "xml")) : null;
        yield return MetadataReference.CreateFromFile(assembly.Location, documentation: documentation);
    }
}
