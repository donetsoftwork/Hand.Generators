using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Hand;

/// <summary>
/// 脚本加载上下文
/// </summary>
public class ScriptLoadContext(bool pdb = false)
    : AssemblyLoadContext, IDisposable
{
    #region 配置
    private readonly bool _pdb = pdb;
    private readonly Dictionary<string, Assembly> _assemblies = [];
    private readonly Dictionary<Assembly, byte[]> _images = [];
    #endregion
    /// <summary>
    /// 获取程序集
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Assembly GetAssembly(CSharpCompilation compilation)
    {
        var assemblyName = compilation.AssemblyName!;
        if (_assemblies.TryGetValue(assemblyName, out var assembly))
            return assembly;
        lock (_assemblies)
        {
            if (_assemblies.TryGetValue(assemblyName, out assembly))
                return assembly;
            assembly = CreateAssembly(compilation);
            _assemblies[assemblyName] = assembly;
            _assemblies[assembly.FullName] = assembly;
            return assembly;
        }
    }
    /// <summary>
    /// 获取程序集
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public Assembly GetAssembly(string assemblyName)
    {
        if (_assemblies.TryGetValue(assemblyName, out var assembly))
            return assembly;
        return Assembly.Load(new AssemblyName(assemblyName));
    }
    /// <summary>
    /// 处理引用
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public MetadataReference? CheckReference(Assembly assembly)
    {
        if (_images.TryGetValue(assembly, out var image))
            return MetadataReference.CreateFromImage(image);
        return null;
    }
    /// <summary>
    /// 构造程序集
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Assembly CreateAssembly(CSharpCompilation compilation)
    {
        var reference = CheckPrevious(compilation);
        if(reference is not null)
            compilation = compilation.AddReferences(reference);
        using var peStream = new MemoryStream();
        using MemoryStream? pdbStream = _pdb ? new MemoryStream() : null;
        var emitResult = compilation.Emit(peStream, pdbStream);
        if (emitResult.Success)
        {
            var assembly = CreateAssembly(peStream, pdbStream);
            _images[assembly] = peStream.ToArray();
            return assembly;
        }
        else
        {
            var diagnostic = emitResult.Diagnostics.FirstOrDefault()
                ?? throw new InvalidOperationException("Compile failed");
            throw new InvalidOperationException($"Compile failed: {diagnostic}");
        }
    }
    /// <summary>
    /// 构造程序集
    /// </summary>
    /// <param name="peStream"></param>
    /// <param name="pdbStream"></param>
    /// <returns></returns>
    private Assembly CreateAssembly(MemoryStream peStream, MemoryStream? pdbStream)
    {
        peStream.Seek(0, SeekOrigin.Begin);
        if (pdbStream is null)
            return LoadFromStream(peStream);
        pdbStream.Seek(0, SeekOrigin.Begin);
        return LoadFromStream(peStream, pdbStream);
    }
    /// <summary>
    /// 检查依赖程序集
    /// </summary>
    /// <param name="compilation"></param>
    private MetadataReference? CheckPrevious(CSharpCompilation compilation)
    {
        var previous = compilation.ScriptCompilationInfo?.PreviousScriptCompilation;
        if (previous is null)
            return null;
        var assembly = GetAssembly(previous);
        return CheckReference(assembly);
    }
    /// <inheritdoc />
    protected override Assembly Load(AssemblyName assemblyName)
    {
        if (_assemblies.TryGetValue(assemblyName.FullName, out var assembly))
            return assembly;
        if (_assemblies.TryGetValue(assemblyName.Name, out var assembly2))
            return assembly2;
        return Assembly.Load(assemblyName);
    }
    void IDisposable.Dispose()
    {
        _assemblies.Clear();
        _images.Clear();
    }
}
