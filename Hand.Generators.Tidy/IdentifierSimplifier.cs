using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Tidy;

/// <summary>
/// 标识名整理器
/// </summary>
/// <param name="container"></param>
public class IdentifierSimplifier(IdentifierNameSyntax container)
    : TypeSimplifier
{
    #region 配置
    private readonly IdentifierNameSyntax _container = container;
    /// <summary>
    /// 容器
    /// </summary>
    public IdentifierNameSyntax Container
        => _container;
    #endregion

    /// <inheritdoc />
    protected override bool CheckGenericCore(ref NameSyntax name)
        => false;
    /// <inheritdoc />
    protected override bool CheckIdentifier(NameSyntax name)
        => TypeComparer.IdentifierEqual(_container, (IdentifierNameSyntax)name);
    /// <inheritdoc />
    protected override bool CheckQualified(ref NameSyntax name)
         => CheckIdentifier(_container, ref name);

    /// <summary>
    /// 使用栈处理标识名
    /// </summary>
    /// <param name="container"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool CheckIdentifier(IdentifierNameSyntax container, ref NameSyntax name)
    {
        var nameQualified = (QualifiedNameSyntax)name;
        var nameLeft = nameQualified.Left;
        var nameRight = nameQualified.Right;
        // 入栈
        var nameStack = CreateStack(ref nameLeft);
        if (nameLeft.Kind() == SyntaxKind.IdentifierName && TypeComparer.IdentifierEqual(container, (IdentifierNameSyntax)nameLeft))
            name = ConcatStack(nameStack, nameRight);
        return false;
    }
}
