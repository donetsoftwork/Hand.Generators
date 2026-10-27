using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Tidy;

/// <summary>
/// 全局格式化
/// </summary>
public class GlobalScope : ITypeSimplifier
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private GlobalScope() { }
    /// <inheritdoc />
    public TypeSyntax Simplify(TypeSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => SyntaxFactory.AliasQualifiedName(TypeComparer.GlobalName, (SimpleNameSyntax)type),
            SyntaxKind.QualifiedName => CheckQualified((QualifiedNameSyntax)type),
            SyntaxKind.AliasQualifiedName => type,
            SyntaxKind.GenericName => SyntaxFactory.AliasQualifiedName(TypeComparer.GlobalName, this.SimplifyArgumentList((GenericNameSyntax)type)),
            SyntaxKind.PredefinedType => type,
            SyntaxKind.ArrayType => this.SimplifyArray((ArrayTypeSyntax)type),
            SyntaxKind.NullableType => this.SimplifyNullable((NullableTypeSyntax)type),
            SyntaxKind.OmittedTypeArgument => type,
            SyntaxKind.TupleType => this.SimplifyTuple((TupleTypeSyntax)type),
            SyntaxKind.PointerType => this.SimplifyPointer((PointerTypeSyntax)type),
            SyntaxKind.FunctionPointerType => this.SimplifyPointer((FunctionPointerTypeSyntax)type),
            SyntaxKind.RefType => this.SimplifyRef((RefTypeSyntax)type),
            SyntaxKind.ScopedType => this.SimplifyScoped((ScopedTypeSyntax)type),
            _ => type,
        };
    }
    /// <inheritdoc cref="Simplify(TypeSyntax)" path="/summary" />
    public NameSyntax Simplify(NameSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => SyntaxFactory.AliasQualifiedName(TypeComparer.GlobalName, (SimpleNameSyntax)type),
            SyntaxKind.QualifiedName => CheckQualified((QualifiedNameSyntax)type),
            SyntaxKind.AliasQualifiedName => type,
            SyntaxKind.GenericName => SyntaxFactory.AliasQualifiedName(TypeComparer.GlobalName, this.SimplifyArgumentList((GenericNameSyntax)type)),
            _ => type,
        };
    }
    /// <summary>
    /// 处理简单类型名
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private SimpleNameSyntax CheckSimple(SimpleNameSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => type,
            SyntaxKind.GenericName => this.SimplifyArgumentList((GenericNameSyntax)type),
            _ => type,
        };
    }
    /// <summary>
    /// 处理限定名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private QualifiedNameSyntax CheckQualified(QualifiedNameSyntax name)
        => SyntaxFactory.QualifiedName(Simplify(name.Left), CheckSimple(name.Right));
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly GlobalScope Instance = new();
}
