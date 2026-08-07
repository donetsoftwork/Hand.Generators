using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 类型
/// </summary>
public partial class SyntaxGenerator
{
    #region PredefinedType
    /// <summary>
    /// bool
    /// </summary>
    public static PredefinedTypeSyntax BoolType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
    /// <summary>
    /// byte
    /// </summary>
    public static PredefinedTypeSyntax ByteType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword));
    /// <summary>
    /// sbyte
    /// </summary>
    public static PredefinedTypeSyntax SByteType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword));
    /// <summary>
    /// int
    /// </summary>
    public static PredefinedTypeSyntax IntType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
    /// <summary>
    /// uint
    /// </summary>
    public static PredefinedTypeSyntax UIntType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword));
    /// <summary>
    /// short
    /// </summary>
    public static PredefinedTypeSyntax ShortType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword));
    /// <summary>
    /// ushort
    /// </summary>
    public static PredefinedTypeSyntax UShortType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword));
    /// <summary>
    /// long
    /// </summary>
    public static PredefinedTypeSyntax LongType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
    /// <summary>
    /// ulong
    /// </summary>
    public static PredefinedTypeSyntax ULongType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword));
    /// <summary>
    /// float
    /// </summary>
    public static PredefinedTypeSyntax FloatType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword));
    /// <summary>
    /// double
    /// </summary>
    public static PredefinedTypeSyntax DoubleType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword));
    /// <summary>
    /// decimal
    /// </summary>
    public static PredefinedTypeSyntax DecimalType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword));
    /// <summary>
    /// string
    /// </summary>
    public static PredefinedTypeSyntax StringType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
    /// <summary>
    /// char
    /// </summary>
    public static PredefinedTypeSyntax CharType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword));
    /// <summary>
    /// DateTime
    /// </summary>
    public static IdentifierNameSyntax DateTimeType => SyntaxFactory.IdentifierName("DateTime");
    /// <summary>
    /// object
    /// </summary>
    public static PredefinedTypeSyntax ObjectType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
    /// <summary>
    /// void
    /// </summary>
    public static PredefinedTypeSyntax VoidType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
    /// <summary>
    /// var
    /// </summary>
    public static IdentifierNameSyntax VarType => SyntaxFactory.IdentifierName("var");
    /// <summary>
    /// IDisposable
    /// </summary>
    public static IdentifierNameSyntax IDisposableType => SyntaxFactory.IdentifierName("IDisposable");
    /// <summary>
    /// Lock
    /// </summary>
    public static TypeSyntax LockType
    {
        get
        {
            //if (_lazyFrameworkMajorVersion.Value >= 9)
            //    return SyntaxFactory.IdentifierName("System.Threading.Lock");
            //return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
            return SyntaxFactory.IdentifierName("System.Threading.Lock");
        }
    }
    /// <summary>
    /// List~1
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public static TypeSyntax ListType(TypeSyntax elementType)
        => SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Collections.Generic.List"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(elementType)));
    /// <summary>
    /// IEnumerable~1
    /// </summary>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public static TypeSyntax IEnumerableType(TypeSyntax elementType)
        => SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Collections.Generic.IEnumerable"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(elementType)));
    #endregion
    #region Generic
    /// <summary>
    /// 泛型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="argumentTypes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GenericNameSyntax Generic(SyntaxToken name, params TypeSyntax[] argumentTypes)
        => SyntaxFactory.GenericName(name, SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(argumentTypes)));
    /// <summary>
    /// 泛型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="argumentTypes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GenericNameSyntax Generic(string name, params TypeSyntax[] argumentTypes)
        => Generic(SyntaxFactory.Identifier(name), argumentTypes);
    #endregion
}
