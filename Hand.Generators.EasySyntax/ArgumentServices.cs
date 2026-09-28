using Hand.Arguments;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 参数扩展方法
/// </summary>
public static partial class GenerateServices
{
    #region ToArgument
    /// <summary>
    /// 转化为参数
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax ToArgument(this ExpressionSyntax argument)
        => SyntaxFactory.Argument(argument);
    /// <summary>
    /// 转化为命名参数
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax ToArgument(this ExpressionSyntax argument, string name)
        => SyntaxFactory.Argument(SyntaxFactory.NameColon(name), default, argument);
    #endregion
    #region ToAttributeArgument
    /// <summary>
    /// 转化常量为特性参数
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeArgumentSyntax ToAttributeArgument(this ExpressionSyntax argument)
        => SyntaxFactory.AttributeArgument(argument);
    /// <summary>
    /// 转化常量为命名特性参数
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeArgumentSyntax ToAttributeArgument(this ExpressionSyntax argument, string name)
        => SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(name), default, argument);
    #endregion
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TArgumentCollection"></typeparam>
    /// <param name="collection"></param>
    /// <param name="argument"></param>
    /// <returns></returns>
    public static TArgumentCollection WithArgument<TArgumentCollection>(this TArgumentCollection collection, ArgumentSyntax argument)
        where TArgumentCollection : IArgumentCollection
    {
        collection.Add(argument);
        return collection;
    }
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TArgumentCollection"></typeparam>
    /// <param name="collection"></param>
    /// <param name="argument"></param>
    /// <returns></returns>
    public static TArgumentCollection WithArgument<TArgumentCollection>(this TArgumentCollection collection, ExpressionSyntax argument)
        where TArgumentCollection : IArgumentCollection
    {
        collection.Add(SyntaxFactory.Argument(argument));
        return collection;
    }
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TArgumentCollection"></typeparam>
    /// <param name="collection"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TArgumentCollection WithArgument<TArgumentCollection>(this TArgumentCollection collection, string name, ExpressionSyntax value)
        where TArgumentCollection : IArgumentCollection
    {
        collection.Add(value.ToArgument(name));
        return collection;
    }
    /// <summary>
    /// in
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax In(this ArgumentSyntax argument)
        => argument.WithRefKindKeyword(_in);
    /// <summary>
    /// ref
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax Ref(this ArgumentSyntax argument)
        => argument.WithRefKindKeyword(_ref);
    /// <summary>
    /// out
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax Out(this ArgumentSyntax argument)
        => argument.WithRefKindKeyword(_out);
}
