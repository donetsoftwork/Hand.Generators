using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Tidy;

/// <summary>
/// 类型简化器
/// </summary>
public class TypeSimplifier
    : ITypeSimplifier
{
    /// <inheritdoc />
    public TypeSyntax Simplify(TypeSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => type,
            SyntaxKind.QualifiedName => SimplifyQualified((NameSyntax)type),
            SyntaxKind.GenericName => this.SimplifyArgumentList((GenericNameSyntax)type),
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
    /// <summary>
    /// 简化限定名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private GenericNameSyntax SimplifyQualified(NameSyntax name)
    {
        CheckQualified(ref name);
        return this.SimplifyArgumentList((GenericNameSyntax)name);
    }
    /// <summary>
    /// 处理类名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Simplify(ref NameSyntax name)
    {
        return name.Kind() switch
        {
            SyntaxKind.IdentifierName => CheckIdentifier(name),
            SyntaxKind.GenericName => CheckGeneric(ref name),
            SyntaxKind.QualifiedName => CheckQualified(ref name),
            _ => false,
        };
    }
    /// <summary>
    /// 处理标识名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected virtual bool CheckIdentifier(NameSyntax name)
        => false;
    /// <summary>
    /// 处理泛型
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool CheckGeneric(ref NameSyntax name)
    {
        var state = CheckGenericCore(ref name);
        name = this.SimplifyArgumentList((GenericNameSyntax)name);
        return state;
    }
    /// <summary>
    /// 处理泛型
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected virtual bool CheckGenericCore(ref NameSyntax name)
        => false;
    /// <summary>
    /// 处理限定名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected virtual bool CheckQualified(ref NameSyntax name)
        => false;

    #region Stack
    /// <summary>
    /// 入栈(拆分为简单名)
    /// </summary>
    /// <param name="left"></param>
    /// <returns></returns>
    protected static Stack<SimpleNameSyntax> CreateStack(ref NameSyntax left)
    {
        var stack = new Stack<SimpleNameSyntax>();
        PushStack(stack, ref left);
        return stack;
    }
    /// <summary>
    /// 入栈(拆分为简单名)
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="left"></param>
    protected static void PushStack(Stack<SimpleNameSyntax> stack, ref NameSyntax left)
    {
        while (left.Kind() == SyntaxKind.QualifiedName)
        {
            var qualified = (QualifiedNameSyntax)left;
            stack.Push(qualified.Right);
            left = qualified.Left;
        }
    }
    /// <summary>
    /// 出栈(拼接为完整名)
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    protected static NameSyntax ConcatStack(Stack<SimpleNameSyntax> stack, SimpleNameSyntax right)
    {
        if (stack.Count == 0)
            return right;
        NameSyntax left = stack.Pop();
        while (stack.Count > 0)
            left = stack.Pop().Qualify(left);
        return right.Qualify(left);
    }
    #endregion
    #region Create
    /// <summary>
    /// 构建类型简化器
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static TypeSimplifier Create(INamespaceSymbol container)
        => container.IsGlobalNamespace ? Inner.Empty : Create(container.ToName());
    /// <summary>
    /// 构建类型简化器
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static TypeSimplifier Create(NameSyntax container)
    {
        switch (container.Kind())
        {
            case SyntaxKind.IdentifierName:
                return new IdentifierSimplifier((IdentifierNameSyntax)container);
            case SyntaxKind.GenericName:
                return new GenericSimplifier((GenericNameSyntax)container);
            default:
                var qualified = (QualifiedNameSyntax)container;
                var left = qualified.Left;
                var rights = QualifiedTidyBase.CreateStack(ref left, qualified.Right);
                if (left.IsKind(SyntaxKind.IdentifierName))
                    return new IdentifierQualifiedSimplifier((IdentifierNameSyntax)left, [.. rights]);
                return new GenericQualifiedSimplifier((GenericNameSyntax)left, [.. rights]);
        }
    }
    /// <summary>
    /// 构建类型简化器
    /// </summary>
    /// <param name="container"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypeSimplifier Create(INamespaceSymbol container, SimpleNameSyntax name)
    {
        if (container.IsGlobalNamespace)
        {
            if (name.IsKind(SyntaxKind.IdentifierName))
                return new IdentifierSimplifier((IdentifierNameSyntax)name);
            return new GenericSimplifier((GenericNameSyntax)name);
        }
        var left = container.ToName();
        var rights = QualifiedTidyBase.CreateStack(ref left, name);
        if (left.IsKind(SyntaxKind.IdentifierName))
            return new IdentifierQualifiedSimplifier((IdentifierNameSyntax)left, [.. rights]);
        return new GenericQualifiedSimplifier((GenericNameSyntax)left, [.. rights]);
    }
    /// <summary>
    /// 构建类型简化器
    /// </summary>
    /// <param name="container"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypeSimplifier Create(NameSyntax container, SimpleNameSyntax name)
    {
        var rights = QualifiedTidyBase.CreateStack(ref container, name);
        if (container.IsKind(SyntaxKind.IdentifierName))
            return new IdentifierQualifiedSimplifier((IdentifierNameSyntax)container, [.. rights]);
        return new GenericQualifiedSimplifier((GenericNameSyntax)container, [.. rights]);
    }
    #endregion
    /// <summary>
    /// 内部延迟加载
    /// </summary>
    class Inner
    {
        internal static readonly TypeSimplifier Empty = new();
    }
}
