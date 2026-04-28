using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hand;

/// <summary>
/// 语法脚本
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="compilation"></param>
/// <param name="loadContext"></param>
public class SyntaxTreeScript<T>(CSharpCompilation compilation, ScriptLoadContext loadContext)
    : SyntaxTreeScript(compilation, loadContext)
{
    /// <summary>
    /// 语法脚本
    /// </summary>
    /// <param name="compilation"></param>
    public SyntaxTreeScript(CSharpCompilation compilation)
        : this(compilation, new ScriptLoadContext())
    {
    }
    #region ExecuteAsync
    /// <summary>
    /// 执行
    /// </summary>
    public async Task<T> ExecuteAsync(object? globals = null, CancellationToken cancellation = default)
    {
        var entryPoint = _compilation.GetEntryPoint(cancellation)
            ?? throw new InvalidOperationException("EntryPoint is not Found");
        var assembly = _loadContext.GetAssembly(_compilation);
        var entryPointMethod = GetEntryMethod(entryPoint, assembly)
            ?? throw new InvalidOperationException("EntryMethod is not Found");
        var submission = entryPointMethod.CreateDelegate(typeof(Func<object[], Task<T>>)) as Func<object[], Task<T>>
            ?? throw new InvalidOperationException("CreateDelegate faild");
        var states = CreateStates(_compilation, globals);
        cancellation.ThrowIfCancellationRequested();
        return await submission(states);
    }
    #endregion

}
