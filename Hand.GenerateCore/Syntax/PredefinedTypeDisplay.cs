using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Syntax;

/// <summary>
/// 包装预定义类型展示
/// </summary>
/// <param name="original"></param>
public class PredefinedTypeDisplay(PredefinedTypeSyntax original)
    : SyntaxWrapper<PredefinedTypeSyntax>(original)
{
    #region PredefinedType
    /// <summary>
    /// bool
    /// </summary>
    public static PredefinedTypeDisplay BoolType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)));
    /// <summary>
    /// byte
    /// </summary>
    public static PredefinedTypeDisplay ByteType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)));
    /// <summary>
    /// sbyte
    /// </summary>
    public static PredefinedTypeDisplay SByteType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword)));
    /// <summary>
    /// int
    /// </summary>
    public static PredefinedTypeDisplay IntType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
    /// <summary>
    /// uint
    /// </summary>
    public static PredefinedTypeDisplay UIntType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword)));
    /// <summary>
    /// short
    /// </summary>
    public static PredefinedTypeDisplay ShortType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword)));
    /// <summary>
    /// ushort
    /// </summary>
    public static PredefinedTypeDisplay UShortType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)));
    /// <summary>
    /// long
    /// </summary>
    public static PredefinedTypeDisplay LongType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)));
    /// <summary>
    /// ulong
    /// </summary>
    public static PredefinedTypeDisplay ULongType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword)));
    /// <summary>
    /// float
    /// </summary>
    public static PredefinedTypeDisplay FloatType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)));
    /// <summary>
    /// double
    /// </summary>
    public static PredefinedTypeDisplay DoubleType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword)));
    /// <summary>
    /// decimal
    /// </summary>
    public static PredefinedTypeDisplay DecimalType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)));
    /// <summary>
    /// string
    /// </summary>
    public static PredefinedTypeDisplay StringType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)));
    /// <summary>
    /// char
    /// </summary>
    public static PredefinedTypeDisplay CharType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword)));
    /// <summary>
    /// object
    /// </summary>
    public static PredefinedTypeDisplay ObjectType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)));
    /// <summary>
    /// void
    /// </summary>
    public static PredefinedTypeDisplay VoidType
        => new(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)));
    #endregion
}
