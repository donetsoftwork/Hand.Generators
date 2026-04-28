using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 修饰扩展方法
/// </summary>
public static partial class GenerateServices
{
    #region SyntaxToken
    internal static readonly SyntaxToken _public = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
    internal static readonly SyntaxToken _private = SyntaxFactory.Token(SyntaxKind.PrivateKeyword);
    internal static readonly SyntaxToken _protected = SyntaxFactory.Token(SyntaxKind.ProtectedKeyword);
    internal static readonly SyntaxToken _internal = SyntaxFactory.Token(SyntaxKind.InternalKeyword);
    internal static readonly SyntaxToken _partial = SyntaxFactory.Token(SyntaxKind.PartialKeyword);
    internal static readonly SyntaxToken _static = SyntaxFactory.Token(SyntaxKind.StaticKeyword);
    internal static readonly SyntaxToken _abstract = SyntaxFactory.Token(SyntaxKind.AbstractKeyword);
    internal static readonly SyntaxToken _virtual = SyntaxFactory.Token(SyntaxKind.VirtualKeyword);
    internal static readonly SyntaxToken _override = SyntaxFactory.Token(SyntaxKind.OverrideKeyword);
    internal static readonly SyntaxToken _async‌ = SyntaxFactory.Token(SyntaxKind.AsyncKeyword);
    internal static readonly SyntaxToken _extern‌ = SyntaxFactory.Token(SyntaxKind.ExternKeyword);
    internal static readonly SyntaxToken _new‌ = SyntaxFactory.Token(SyntaxKind.NewKeyword);
    internal static readonly SyntaxToken _sealed‌ = SyntaxFactory.Token(SyntaxKind.SealedKeyword);
    internal static readonly SyntaxToken _readonly = SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword);
    internal static readonly SyntaxToken _volatile‌ = SyntaxFactory.Token(SyntaxKind.VolatileKeyword);
    internal static readonly SyntaxToken _const = SyntaxFactory.Token(SyntaxKind.ConstKeyword);
    internal static readonly SyntaxToken _params = SyntaxFactory.Token(SyntaxKind.ParamsKeyword);
    internal static readonly SyntaxToken _in = SyntaxFactory.Token(SyntaxKind.InKeyword);
    internal static readonly SyntaxToken _ref = SyntaxFactory.Token(SyntaxKind.RefKeyword);
    internal static readonly SyntaxToken _out = SyntaxFactory.Token(SyntaxKind.OutKeyword);
    #endregion
    #region Modifiers
    /// <summary>
    /// 修饰
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="modifier"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Modify<TDeclarationSyntax>(this TDeclarationSyntax declaration, SyntaxToken modifier)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => (TDeclarationSyntax)declaration.WithModifiers(declaration.Modifiers.Add(modifier));
    /// <summary>
    /// 修饰
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="modifiers"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Modify<TDeclarationSyntax>(this TDeclarationSyntax declaration, params IEnumerable<SyntaxToken> modifiers)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => (TDeclarationSyntax)declaration.WithModifiers(declaration.Modifiers.AddRange(modifiers));
    /// <summary>
    /// 修饰
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Modify<TDeclarationSyntax>(this TDeclarationSyntax declaration, SyntaxKind kind)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(SyntaxFactory.Token(kind));
    /// <summary>
    /// 公开
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Public<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_public);
    /// <summary>
    /// 私有
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Private<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_private);
    /// <summary>
    /// 保护
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Protected<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_protected);
    /// <summary>
    /// 内部
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Internal<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_internal);
    /// <summary>
    /// 部分
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Partial<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_partial);
    /// <summary>
    /// 抽象
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Abstract<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_abstract);
    /// <summary>
    /// 虚
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TMethod Virtual<TMethod>(this TMethod method)
        where TMethod : BaseMethodDeclarationSyntax
        => method.Modify(_virtual);
    /// <summary>
    /// 重写
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TMethod Override<TMethod>(this TMethod method)
        where TMethod : BaseMethodDeclarationSyntax
        => method.Modify(_override);
    /// <summary>
    /// 异步
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TMethod Async<TMethod>(this TMethod method)
        where TMethod : BaseMethodDeclarationSyntax
        => method.Modify(_async);
    /// <summary>
    /// 外部
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TMethod Extern<TMethod>(this TMethod method)
        where TMethod : BaseMethodDeclarationSyntax
        => method.Modify(_extern);
    /// <summary>
    /// 隐藏
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax New<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_new‌);
    /// <summary>
    /// 密封
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Sealed<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_sealed‌);
    /// <summary>
    /// 静态
    /// </summary>
    /// <typeparam name="TDeclarationSyntax"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclarationSyntax Static<TDeclarationSyntax>(this TDeclarationSyntax declaration)
        where TDeclarationSyntax : MemberDeclarationSyntax
        => declaration.Modify(_static);
    /// <summary>
    /// 只读
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax ReadOnly(this FieldDeclarationSyntax field)
        => field.Modify(_readonly);
    /// <summary>
    /// 易变(多线程访问和修改)
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Volatile(this FieldDeclarationSyntax field)
        => field.Modify(_volatile);
    /// <summary>
    /// 常量
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Const(this FieldDeclarationSyntax field)
        => field.Modify(_const);
    /// <summary>
    /// params修饰符
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParameter Params<TParameter>(this TParameter parameter)
        where TParameter : BaseParameterSyntax
        => (TParameter)parameter.WithModifiers(parameter.Modifiers.Add(_params));
    /// <summary>
    /// in修饰符
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParameter In<TParameter>(this TParameter parameter)
        where TParameter : BaseParameterSyntax
        => (TParameter)parameter.WithModifiers(parameter.Modifiers.Add(_in));
    /// <summary>
    /// ref修饰符
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParameter Ref<TParameter>(this TParameter parameter)
        where TParameter : BaseParameterSyntax
        => (TParameter)parameter.WithModifiers(parameter.Modifiers.Add(_ref));
    /// <summary>
    /// out修饰符
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TParameter Out<TParameter>(this TParameter parameter)
        where TParameter : BaseParameterSyntax
        => (TParameter)parameter.WithModifiers(parameter.Modifiers.Add(_out));
    #endregion
    /// <summary>
    /// 是否为partial
    /// </summary>
    /// <param name="modifiers"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartial(this SyntaxTokenList modifiers)
        => modifiers.Any(SyntaxKind.PartialKeyword);
    /// <summary>
    /// 是否为静态
    /// </summary>
    /// <param name="modifiers"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStatic(this SyntaxTokenList modifiers)
         => modifiers.Any(SyntaxKind.StaticKeyword);
    /// <summary>
    /// 是否包含类型
    /// </summary>
    /// <param name="modifiers"></param>
    /// <param name="kinds"></param>
    /// <returns></returns>
    public static bool HasKinds(this SyntaxTokenList modifiers, params SyntaxKind[] kinds)
    {
        var state = true;
        foreach (var kind in kinds)
        {
            if(modifiers.Any(kind)) 
                return true;
            else
                state = false;
        }
        return state;
    }
}
