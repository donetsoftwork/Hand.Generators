using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
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
    /// 转化为程序集引用
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<MetadataReference> ToReferences(this Assembly assembly)
    {
        if (assembly.IsDynamic)
            yield break;
        var location = assembly.Location;
        if (string.IsNullOrEmpty(location))
            yield break;
        yield return MetadataReference.CreateFromFile(assembly.Location);
    }
}
