using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Tidy;

/// <summary>
/// 标识名开头的限定名简化器
/// </summary>
/// <param name="root"></param>
/// <param name="rights"></param>
public class IdentifierQualifiedSimplifier(/*QualifiedNameSyntax container, */IdentifierNameSyntax root, SimpleNameSyntax[] rights)
    : QualifiedTidyBase(/*container, */rights)
{
    #region 配置
    private readonly IdentifierNameSyntax _root = root;
    /// <summary>
    /// 根
    /// </summary>
    public IdentifierNameSyntax Root 
        => _root;
    #endregion

    /// <inheritdoc />
    protected override bool CheckGenericCore(ref NameSyntax name)
        => false;
    /// <inheritdoc />
    protected override bool CheckIdentifier(NameSyntax name)
        => TypeComparer.IdentifierEqual(_root, (IdentifierNameSyntax)name);
    /// <inheritdoc />
    protected override bool CheckQualified(ref NameSyntax name)
        => CheckQualified(_root, _rights, ref name);
}
