using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 语句扩展方法
/// </summary>
public static partial class GenerateServices
{
    /// <summary>
    /// 添加
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="statement"></param>
    /// <returns></returns>
    public static TCollect Add<TCollect>(this TCollect collect, StatementSyntax statement)
        where TCollect : StatementCollect
    {
        collect.AddCore(statement);
        return collect;
    }
    /// <summary>
    /// 添加
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect AddExpression<TCollect>(this TCollect collect, ExpressionSyntax expression)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.ExpressionStatement(expression));
    #region Declare
    /// <summary>
    /// 添加变量声明
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="variable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Declare<TCollect>(this TCollect collect, VariableDeclarationSyntax variable)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.LocalDeclarationStatement(variable));
    /// <summary>
    /// 添加变量声明
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="variableType"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Declare<TCollect>(this TCollect collect, TypeSyntax variableType, SyntaxToken variableName)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.LocalDeclarationStatement(variableType.Variable(variableName)));
    /// <summary>
    /// 添加变量声明
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="variableType"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Declare<TCollect>(this TCollect collect, TypeSyntax variableType, IdentifierNameSyntax variableName)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.LocalDeclarationStatement(variableType.Variable(variableName.Identifier)));
    #endregion
    #region Goto
    /// <summary>
    /// goto
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="labelName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Goto<TCollect>(this TCollect collect, SyntaxToken labelName)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.GotoStatement(SyntaxKind.GotoStatement, SyntaxFactory.IdentifierName(labelName)));
    /// <summary>
    /// goto
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <param name="labelName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Goto<TCollect>(this TCollect collect, string labelName)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.GotoStatement(SyntaxKind.GotoStatement, SyntaxFactory.IdentifierName(labelName)));
    #endregion
    /// <summary>
    /// break
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Break<TCollect>(this TCollect collect)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.BreakStatement());
    /// <summary>
    /// continue
    /// </summary>
    /// <typeparam name="TCollect"></typeparam>
    /// <param name="collect"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TCollect Continue<TCollect>(this TCollect collect)
        where TCollect : StatementCollect
        => collect.Add(SyntaxFactory.ContinueStatement());
    /// <summary>
    /// Block
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BlockBuilder<TParent, StatementBuilder<TParent>> Block<TParent>(this StatementBuilder<TParent> builder)
        => new(builder);
    /// <summary>
    /// If
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IfBuilder<TParent, StatementBuilder<TParent>> If<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax condition)
        => new(builder, new IfBuilder(condition));
    /// <summary>
    /// If
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IfBuilder If(this ExpressionSyntax condition)
        => new(condition);
    /// <summary>
    /// switch语句
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="governing"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SwitchBuilder<TParent, StatementBuilder<TParent>> Switch<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax governing)
        => new(builder, new SwitchBuilder(governing));
    /// <summary>
    /// switch语句
    /// </summary>
    /// <param name="governing"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SwitchBuilder Switch(this ExpressionSyntax governing)
        => new(governing);
    /// <summary>
    /// switch表达式
    /// </summary>
    /// <param name="governing"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PatternSwitchBuilder SwitchExpression(this ExpressionSyntax governing)
        => new(governing);
    /// <summary>
    /// 结束分支
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParent End<TParent>(this StatementBuilder<TParent> builder)
        => builder.BuildCore();
    #region ForEach
    /// <summary>
    /// foreach
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="itemType"></param>
    /// <param name="item"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForEachBuilder<TParent, StatementBuilder<TParent>> ForEach<TParent>(this StatementBuilder<TParent> builder, TypeSyntax itemType, SyntaxToken item, ExpressionSyntax collection)
        => new(builder, new ForEachBuilder(itemType, item, collection));
    /// <summary>
    /// foreach
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="item"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForEachBuilder<TParent, StatementBuilder<TParent>> ForEach<TParent>(this StatementBuilder<TParent> builder, SyntaxToken item, ExpressionSyntax collection)
        => new(builder, new ForEachBuilder(SyntaxGenerator.VarType, item, collection));
    /// <summary>
    /// foreach
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForEachBuilder<TParent, StatementBuilder<TParent>> ForEach<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax collection)
        => new(builder, new ForEachBuilder(SyntaxGenerator.VarType, SyntaxFactory.Identifier("item"), collection));
    /// <summary>
    /// foreach
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="itemType"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForEachBuilder ForEach(this ExpressionSyntax collection, TypeSyntax itemType, SyntaxToken item)
        => new(itemType, item, collection);
    /// <summary>
    /// foreach
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForEachBuilder ForEach(this ExpressionSyntax collection, SyntaxToken item)
        => new(SyntaxGenerator.VarType, item, collection);
    /// <summary>
    /// foreach
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForEachBuilder ForEach(this ExpressionSyntax collection)
        => new(SyntaxGenerator.VarType, SyntaxFactory.Identifier("item"), collection);
    #endregion
    #region For
    /// <summary>
    /// for
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="declaration"></param>
    /// <param name="condition"></param>
    /// <param name="incrementors"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder<TParent, StatementBuilder<TParent>> For<TParent>(this StatementBuilder<TParent> builder, VariableDeclarationSyntax declaration, ExpressionSyntax condition, List<ExpressionSyntax> incrementors)
        => new(builder, new ForBuilder(declaration, condition, incrementors));
    /// <summary>
    /// for
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="declaration"></param>
    /// <param name="condition"></param>
    /// <param name="incrementor"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder<TParent, StatementBuilder<TParent>> For<TParent>(this StatementBuilder<TParent> builder, VariableDeclarationSyntax declaration, ExpressionSyntax condition, ExpressionSyntax incrementor)
        => new(builder, new ForBuilder(declaration, condition, [incrementor]));
    /// <summary>
    /// for
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="index"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder<TParent, StatementBuilder<TParent>> For<TParent>(this StatementBuilder<TParent> builder, IdentifierNameSyntax index, ExpressionSyntax limit)
        => new(builder, For(limit, index));
    /// <summary>
    /// for
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="index"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder<TParent, StatementBuilder<TParent>> For<TParent>(this StatementBuilder<TParent> builder, SyntaxToken index, ExpressionSyntax limit)
        => For(builder, index.ToIdentifierName(), limit);
    /// <summary>
    /// for
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="index"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder<TParent, StatementBuilder<TParent>> For<TParent>(this StatementBuilder<TParent> builder, string index, ExpressionSyntax limit)
        => For(builder, SyntaxFactory.IdentifierName(index), limit);
    /// <summary>
    /// for
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder<TParent, StatementBuilder<TParent>> For<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax limit)
        => For(builder, SyntaxFactory.IdentifierName("i"), limit);
    /// <summary>
    /// for
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="declaration"></param>
    /// <param name="incrementors"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder For(this ExpressionSyntax condition, VariableDeclarationSyntax declaration, List<ExpressionSyntax> incrementors)
        => new(declaration, condition, incrementors);
    /// <summary>
    /// for
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="declaration"></param>
    /// <param name="incrementor"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder For(this ExpressionSyntax condition, VariableDeclarationSyntax declaration, ExpressionSyntax incrementor)
        => new(declaration, condition, [incrementor]);
    /// <summary>
    /// for
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="index"></param>
    /// <returns></returns>    
    public static ForBuilder For(this ExpressionSyntax limit, IdentifierNameSyntax index)
    {
        // var index = 0
        var declaration = SyntaxGenerator.VarType.Variable(index.Identifier, SyntaxGenerator.Literal(0));
        // index < limit
        var condition = index.LessThan(limit);
        // index++
        var incrementor = index.PostIncrement();
        return new(declaration, condition, [incrementor]);
    }
    /// <summary>
    /// for
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ForBuilder For(this ExpressionSyntax limit, SyntaxToken index)
        => For(limit, SyntaxFactory.IdentifierName(index));
    /// <summary>
    /// for
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ForBuilder For(this ExpressionSyntax limit, string index)
        => For(limit, SyntaxFactory.IdentifierName(index));
    /// <summary>
    /// for
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ForBuilder For(this ExpressionSyntax limit)
        => For(limit, SyntaxFactory.IdentifierName("i"));
    #endregion
    /// <summary>
    /// while
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WhileBuilder<TParent, StatementBuilder<TParent>> While<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax condition)
        => new(builder, new WhileBuilder(condition));
    /// <summary>
    /// while
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WhileBuilder While(this ExpressionSyntax condition)
        => new(condition);
    /// <summary>
    /// do while
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoBuilder<TParent, StatementBuilder<TParent>> Do<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax condition)
        => new(builder, new DoBuilder(condition));
    /// <summary>
    /// do while
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoBuilder Do(this ExpressionSyntax condition)
        => new(condition);
    /// <summary>
    /// lock
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LockBuilder<TParent, StatementBuilder<TParent>> Lock<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax expression)
        => new(builder, new LockBuilder(expression));
    /// <summary>
    /// lock
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LockBuilder Lock(this ExpressionSyntax expression)
        => new(expression);
    /// <summary>
    /// try
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TryBuilder<TParent, StatementBuilder<TParent>> Try<TParent>(this StatementBuilder<TParent> builder)
        => new(builder, new TryBuilder());
    #region Return
    /// <summary>
    /// 返回
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static TParent Return<TParent>(this StatementBuilder<TParent> builder, ExpressionSyntax? expression = null)
    {
        builder.Add(SyntaxFactory.ReturnStatement(expression));
        return builder.BuildCore();
    }
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="variable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParent Return<TParent>(this StatementBuilder<TParent> builder, SyntaxToken variable)
        => Return(builder, SyntaxFactory.IdentifierName(variable));
    #endregion
    /// <summary>
    /// 返回true
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParent ReturnTrue<TParent>(this StatementBuilder<TParent> builder)
        => builder.Return(SyntaxGenerator.TrueLiteral);
    /// <summary>
    /// 返回false
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParent ReturnFalse<TParent>(this StatementBuilder<TParent> builder)
        => builder.Return(SyntaxGenerator.FalseLiteral);
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="builder"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static TParent Throw<TParent>(this StatementBuilder<TParent> builder, ThrowExpressionSyntax exception)
    {
        builder.Add(SyntaxFactory.ExpressionStatement(exception));
        return builder.BuildCore();
    }
}
