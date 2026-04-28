using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hand;

/// <summary>
/// 语法树执行器
/// </summary>
public partial class SyntaxTreeDriver
{
    #region Parse
    /// <summary>
    /// 解析源码
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public SyntaxTree Parse(string source)
    {
        return CSharpSyntaxTree.ParseText(source, path: _path, options: _options)
            .GetCompilationUnitRoot()
            .Using(_usings)
            .SyntaxTree;
    }
    #endregion
    /// <summary>
    /// 新建程序集名称
    /// </summary>
    /// <returns></returns>
    private string NewAssemblyName(string prefix)
        => prefix + Guid.NewGuid().ToString("N");
    /// <summary>
    /// 编译配置
    /// </summary>
    /// <returns></returns>
    public CSharpCompilationOptions GetCompilationOptions()
    {
        return new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            sourceReferenceResolver: new SourceFileResolver
            (
                searchPaths: [_path],
                baseDirectory: _path
            ));
    }
    #region Compile
    /// <summary>
    /// 编译
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public CSharpCompilation Compile(string source)
        => Compile(Parse(source));
    /// <summary>
    /// 编译
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <returns></returns>
    public CSharpCompilation Compile(SyntaxTree syntaxTree)
    {
        return CSharpCompilation.Create(
            NewAssemblyName("Hand.Compilation"),
            [syntaxTree],
            _references,
            GetCompilationOptions());
    }
    #endregion
    #region ScriptCompile
    /// <summary>
    /// 编译脚本
    /// </summary>
    /// <param name="source"></param>
    /// <param name="previous"></param>
    /// <param name="returnType"></param>
    /// <param name="globalsType"></param>
    /// <returns></returns>
    public CSharpCompilation ScriptCompile(
        string source,
        CSharpCompilation? previous = null,
        Type? returnType = null,
        Type? globalsType = null)
    {
        return ScriptCompile(
            Parse(source),
            previous,
            returnType,
            globalsType);
    }
    /// <summary>
    /// 编译脚本
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="previous"></param>
    /// <param name="returnType"></param>
    /// <param name="globalsType"></param>
    /// <returns></returns>
    public CSharpCompilation ScriptCompile(
        SyntaxTree syntaxTree,
        CSharpCompilation? previous = null,
        Type? returnType = null,
        Type? globalsType = null)
    {
        return CSharpCompilation.CreateScriptCompilation(
            NewAssemblyName("Hand.Script"),
            CheckScript(syntaxTree),
            _references,
            GetCompilationOptions(),
            previous,
            returnType,
            globalsType);
    }
    #endregion
    #region CreateScript
    /// <summary>
    /// 构造脚本
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="previous"></param>
    /// <param name="globalsType"></param>
    /// <returns></returns>
    public SyntaxTreeScript<TResult> CreateScript<TResult>(
        string source,
        CSharpCompilation? previous = null,
        Type? globalsType = null)
    {
        return ScriptCompile(Parse(source), previous, typeof(TResult), globalsType)
            .ToScript<TResult>();
    }
    /// <summary>
    /// 构造脚本
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="syntaxTree"></param>
    /// <param name="previous"></param>
    /// <param name="globalsType"></param>
    /// <returns></returns>
    public SyntaxTreeScript<TResult> CreateScript<TResult>(
        SyntaxTree syntaxTree,
        CSharpCompilation? previous = null,
        Type? globalsType = null)
    {
        return ScriptCompile(syntaxTree, previous, typeof(TResult), globalsType)
            .ToScript<TResult>();
    }
    #endregion
    #region ExecuteAsync
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellation"></param>
    /// <param name="previous"></param>
    /// <param name="globals"></param>
    /// <param name="globalsType"></param>
    /// <returns></returns>
    public Task<T> ExecuteAsync<T>(
        string source,
        CancellationToken cancellation = default,
        CSharpCompilation? previous = null,
        object? globals = null,
        Type? globalsType = null)
    {
        return ExecuteAsync<T>(Parse(source), cancellation, previous, globals, globalsType);
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="cancellation"></param>
    /// <param name="previous"></param>
    /// <param name="globals"></param>
    /// <param name="globalsType"></param>
    /// <returns></returns>
    public Task<T> ExecuteAsync<T>(
        SyntaxTree syntaxTree,
        CancellationToken cancellation = default,
        CSharpCompilation? previous = null,
        object? globals = null,
        Type? globalsType = null)
    {
        return CreateScript<T>(syntaxTree, previous, globalsType ?? globals?.GetType())
            .ExecuteAsync(globals, cancellation);
    }
    #endregion
    /// <summary>
    /// 处理脚本语法树
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <returns></returns>
    public static SyntaxTree CheckScript(SyntaxTree syntaxTree)
    {
        var options = syntaxTree.Options;
        if (options.Kind == SourceCodeKind.Script)
            return syntaxTree;
        return syntaxTree.WithRootAndOptions(syntaxTree.GetRoot(), options.WithKind(SourceCodeKind.Script));
    }
}