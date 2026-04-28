using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Hand;

/// <summary>
/// 语法脚本
/// </summary>
/// <param name="compilation"></param>
/// <param name="loadContext"></param>
public abstract class SyntaxTreeScript(CSharpCompilation compilation, ScriptLoadContext loadContext)
{
    #region 配置
    /// <summary>
    /// 编译信息
    /// </summary>
    protected readonly CSharpCompilation _compilation = compilation;
    /// <summary>
    /// 脚本加载上下文
    /// </summary>
    protected readonly ScriptLoadContext _loadContext = loadContext;
    /// <summary>
    /// 编译信息
    /// </summary>
    public CSharpCompilation Compilation => _compilation;
    /// <summary>
    /// 脚本加载上下文
    /// </summary>
    public ScriptLoadContext LoadContext 
        => _loadContext;
    #endregion
    /// <summary>
    /// 构造状态数组
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="globals"></param>
    /// <returns></returns>
    public static object[] CreateStates(CSharpCompilation compilation, object? globals)
    {
        var states = new object[CheckStateCount(compilation)];
        if (globals is not null)
            states[0] = globals;
        return states;
    }
    /// <summary>
    /// 状态计数检查
    /// </summary>
    /// <param name="submissionStateCount"></param>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static int CheckStateCount(CSharpCompilation compilation, int submissionStateCount = 2)
    {
        var previous = compilation.ScriptCompilationInfo?.PreviousScriptCompilation;
        if (previous is null)
            return submissionStateCount;
        return CheckStateCount(previous, submissionStateCount + 1);
    }
    /// <summary>
    /// 获取入口方法
    /// </summary>
    /// <param name="entryPoint"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static MethodInfo? GetEntryMethod(IMethodSymbol entryPoint, Assembly assembly)
    {
        string entryPointTypeName = CheckQualifiedName(entryPoint.ContainingNamespace.MetadataName, entryPoint.ContainingType.MetadataName);
        string entryPointMethodName = entryPoint.MetadataName;

        var entryPointType = assembly.GetType(entryPointTypeName, throwOnError: true, ignoreCase: false)?
            .GetTypeInfo();
        if (entryPointType == null)
            return null;
        return entryPointType.GetDeclaredMethod(entryPointMethodName);
    }
    /// <summary>
    /// 处理限定名称
    /// </summary>
    /// <param name="qualifier"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string CheckQualifiedName(string? qualifier, string name)
    {
        if (string.IsNullOrEmpty(qualifier))
            return name;
        return string.Concat(qualifier, ".", name);
    }
}
