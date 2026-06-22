using Hand.Members;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Hand.Extensions;

/// <summary>
/// 扩展方法服务
/// </summary>
/// <param name="compilation"></param>
/// <param name="info"></param>
/// <param name="isInternal"></param>
public class ExtensionService(Compilation compilation, TypeNameInfo info, bool isInternal)
{
    #region 配置
    private readonly Compilation _compilation = compilation;
    private readonly TypeNameInfo _info = info;
    private readonly INamedTypeSymbol? _symbol = compilation.GetTypeByMetadataName(info.FullName);
    private readonly bool _isInternal = isInternal;

    /// <summary>
    /// 编译对象
    /// </summary>
    public Compilation Compilation
        => _compilation;
    /// <summary>
    /// 扩展类信息
    /// </summary>
    public TypeNameInfo Info 
        => _info;
    /// <summary>
    /// 扩展类符号
    /// </summary>
    public INamedTypeSymbol? Symbol
        => _symbol;
    /// <summary>
    /// 是否为internal
    /// </summary>
    public bool IsInternal 
        => _isInternal;
    #endregion

    /// <summary>
    /// 获取扩展方法
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IMethodSymbol> GetMethods()
    {
        if (_symbol == null)
            return [];
        return SymbolReflection.GetMethods(_symbol);
    }

}
