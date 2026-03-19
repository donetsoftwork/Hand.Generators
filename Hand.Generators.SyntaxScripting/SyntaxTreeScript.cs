using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hand;

/// <summary>
/// 语法树执行器
/// </summary>
public partial class SyntaxTreeScript
{
    #region Parse
    /// <summary>
    /// 解析源码
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public SyntaxTree Parse(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: _path, options: _options);
        return syntaxTree.GetRoot()
            .Using(_usings)
            .SyntaxTree;
    }
    #endregion
    #region Compile
    /// <summary>
    /// 编译
    /// </summary>
    /// <param name="source"></param>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public Compilation Compile(string source, string? assemblyName = "<SyntaxTreeScript>")
        => Compile(Parse(source), assemblyName);
    /// <summary>
    /// 编译
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public Compilation Compile(SyntaxTree syntaxTree, string? assemblyName = "<SyntaxTreeScript>")
    {
        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            sourceReferenceResolver: new SourceFileResolver
            (
                searchPaths: [_path],
                baseDirectory: _path
            ));
        return CSharpCompilation.Create(assemblyName,
            [syntaxTree],
            _references,
            compilationOptions);
    }
    #endregion
    
}