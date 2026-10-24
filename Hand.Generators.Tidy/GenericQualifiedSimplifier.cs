using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Tidy;

/// <summary>
/// 标识名开头的限定名简化器
/// </summary>
/// <param name="root"></param>
/// <param name="parts"></param>
public class GenericQualifiedSimplifier(/*QualifiedNameSyntax container, */GenericNameSyntax root, SimpleNameSyntax[] parts)
    : QualifiedTidyBase(/*container, */parts)
{
    #region 配置
    private readonly GenericNameSyntax _root = root;
    /// <summary>
    /// 根
    /// </summary>
    public GenericNameSyntax Root
        => _root;
    #endregion

    /// <inheritdoc />
    protected override bool CheckGenericCore(ref NameSyntax name)
        => TypeComparer.GenericEqual(_root, (GenericNameSyntax)name);
    /// <inheritdoc />
    protected override bool CheckIdentifier(NameSyntax name)
        => false;
    /// <inheritdoc />
    protected override bool CheckQualified(ref NameSyntax name)
        => CheckQualified(_root, _rights, ref name);
}
