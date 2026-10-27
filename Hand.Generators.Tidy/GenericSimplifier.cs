using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Tidy;

/// <summary>
/// 泛型整理器
/// </summary>
/// <param name="container"></param>
public class GenericSimplifier(GenericNameSyntax container)
    : TypeSimplifier
{
    #region 配置
    private readonly GenericNameSyntax _container = container;
    /// <summary>
    /// 容器
    /// </summary>
    public GenericNameSyntax Container
        => _container;
    #endregion

    /// <inheritdoc />
    protected override bool CheckGenericCore(ref NameSyntax name)
        => TypeComparer.GenericEqual(_container, (GenericNameSyntax)name);
    ///// <inheritdoc />
    //protected override bool CheckIdentifier(NameSyntax name)
    //    => false;
    /// <inheritdoc />
    protected override bool CheckQualified(ref NameSyntax name)
         => CheckGeneric(_container, ref name);

    /// <summary>
    /// 使用栈处理泛型
    /// </summary>
    /// <param name="container"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool CheckGeneric(GenericNameSyntax container, ref NameSyntax name)
    {
        var nameQualified = (QualifiedNameSyntax)name;
        var nameLeft = nameQualified.Left;
        var nameRight = nameQualified.Right;
        // 入栈
        var nameStack = CreateStack(ref nameLeft);
        if (nameLeft.Kind() == SyntaxKind.GenericName && TypeComparer.GenericEqual(container, (GenericNameSyntax)nameLeft))
            name = ConcatStack(nameStack, nameRight);
        return false;
    }
}
