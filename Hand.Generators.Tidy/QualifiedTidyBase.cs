using Hand.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.Tidy;

/// <summary>
/// 限定名整理基类
/// </summary>
/// <param name="rights"></param>
public abstract class QualifiedTidyBase(SimpleNameSyntax[] rights)
    : TypeSimplifier
{
    #region 配置
    /// <summary>
    /// 右侧
    /// </summary>
    protected readonly SimpleNameSyntax[] _rights = rights;
    /// <summary>
    /// 右侧
    /// </summary>
    public IEnumerable<SimpleNameSyntax> Rights
        => _rights;
    #endregion

    /// <summary>
    /// 处理限定名
    /// </summary>
    /// <param name="containerSimple"></param>
    /// <param name="containerParts"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    protected static bool CheckQualified(SimpleNameSyntax containerSimple, SimpleNameSyntax[] containerParts, ref NameSyntax name)
    {
        var nameQualified = (QualifiedNameSyntax)name;
        var nameLeft = nameQualified.Left;
        var nameStack = CreateStack(ref nameLeft, nameQualified.Right);
        var nameSimple = (SimpleNameSyntax)nameLeft;
        var containerIndex = 0;
        var containerLimit = Math.Min(containerParts.Length, nameStack.Count);
        bool state;
        while (state = CheckSimple(containerSimple, nameSimple))
        {
            if (containerIndex >= containerLimit)
                break;
            containerSimple = containerParts[containerIndex++];
            nameSimple = nameStack.Pop();
        }
        if (state)
        {
            if (nameStack.Count == 0)
            {
                name = nameSimple;
                return containerIndex >= containerLimit;
            }
            nameSimple = nameStack.Pop();
        }
        name = ConcatStack(nameSimple, nameStack);
        return false;
    }
    /// <summary>
    /// 比较简单名
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static bool CheckSimple(SimpleNameSyntax scope, SimpleNameSyntax name)
    {
        var kind = scope.Kind();
        if (kind == name.Kind())
        {
            if (kind == SyntaxKind.IdentifierName)
                return TypeComparer.IdentifierEqual((IdentifierNameSyntax)scope, (IdentifierNameSyntax)name);
            else if (kind == SyntaxKind.GenericName)
                return TypeComparer.GenericEqual((GenericNameSyntax)scope, (GenericNameSyntax)name);
        }
        return false;
    }
    /// <summary>
    /// 入栈(拆分为简单名)
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    internal static Stack<SimpleNameSyntax> CreateStack(ref NameSyntax left, SimpleNameSyntax right)
    {
        var stack = new Stack<SimpleNameSyntax>();
        stack.Push(right);
        PushStack(stack, ref left);
        return stack;
    }
    /// <summary>
    /// 出栈(拼接为完整名)
    /// </summary>
    /// <param name="left"></param>
    /// <param name="stack"></param>
    /// <returns></returns>
    private static NameSyntax ConcatStack(NameSyntax left, Stack<SimpleNameSyntax> stack)
    {
        while (stack.Count > 0)
            left = stack.Pop().Qualify(left);
        return left;
    }
}
