// 正常情况下该逻辑应该不会命中, ClassifyCommonConversion逻辑会覆盖
//using Hand.Converters;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System.Collections.Generic;

//namespace Hand.Enums;

///// <summary>
///// 从基础类型转化为枚举
///// </summary>
//public class EnumToObjectConverter(TypeSyntax enumType)
//    : StaticMethodConverter(_parse)
//{
//    #region 配置
//    private static readonly IdentifierNameSyntax _parse = SyntaxFactory.IdentifierName("System.Enum.ToObject");
//    /// <summary>
//    /// 支持的来源类型
//    /// </summary>
//    private static readonly HashSet<SpecialType> _supportedTypes =
//    [
//        SpecialType.System_SByte,
//        SpecialType.System_Byte,
//        SpecialType.System_Int16,
//        SpecialType.System_UInt16,
//        SpecialType.System_Int32,
//        SpecialType.System_UInt32,
//        SpecialType.System_Int64,
//        SpecialType.System_UInt64
//    ];
//    private readonly TypeSyntax _enumType = enumType;
//    private readonly TypeOfExpressionSyntax _enumTypeArgument = enumType.TypeOf();

//    /// <summary>
//    /// 枚举类型
//    /// </summary>
//    public TypeSyntax EnumType
//        => _enumType;
//    #endregion
//    /// <inheritdoc />
//    public override ExpressionSyntax Convert(ExpressionSyntax source)
//    {
//        // (TEnum)System.Enum.ToObject(typeof(TEnum), value)
//        return _enumType.Cast(base.Convert(source));
//    }
//    /// <inheritdoc />
//    protected override IEnumerable<ExpressionSyntax> CreateArguments(ExpressionSyntax source)
//        => [_enumTypeArgument, source];
//    /// <summary>
//    /// 判断是否支持转化为枚举
//    /// </summary>
//    /// <param name="specialType"></param>
//    /// <returns></returns>
//    public static bool CheckSupported(SpecialType specialType)
//        => _supportedTypes.Contains(specialType);
//}
