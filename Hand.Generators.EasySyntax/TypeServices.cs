using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 预定义类型扩展方法
/// </summary>
public static partial class GenerateServices
{
    /// <summary>
    /// 是bool类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBool(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsBool(predefined);
    /// <summary>
    /// 是bool类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBool(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.BoolKeyword);
    /// <summary>
    /// 是byte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsByte(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsByte(predefined);
    /// <summary>
    /// 是byte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsByte(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ByteKeyword);
    /// <summary>
    /// 是sbyte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSByte(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsSByte(predefined);
    /// <summary>
    /// 是sbyte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSByte(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.SByteKeyword);
    /// <summary>
    /// 是Int类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsInt(predefined);
    /// <summary>
    /// 是Int类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.IntKeyword);
    /// <summary>
    /// 是uint类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUInt(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsUInt(predefined);
    /// <summary>
    /// 是uint类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUInt(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.UIntKeyword);
    /// <summary>
    /// 是short类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsShort(predefined);
    /// <summary>
    /// 是short类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ShortKeyword);
    /// <summary>
    /// 是UShort类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUShort(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsUShort(predefined);
    /// <summary>
    /// 是UShort类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUShort(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.UShortKeyword);
    /// <summary>
    /// 是long类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLong(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsLong(predefined);
    /// <summary>
    /// 是long类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLong(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.LongKeyword);
    /// <summary>
    /// 是ulong类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsULong(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsULong(predefined);
    /// <summary>
    /// 是ulong类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsULong(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ULongKeyword);
    /// <summary>
    /// 是float类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFloat(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsFloat(predefined);
    /// <summary>
    /// 是float类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFloat(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.FloatKeyword);
    /// <summary>
    /// 是double类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDouble(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsDouble(predefined);
    /// <summary>
    /// 是double类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDouble(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.DoubleKeyword);
    /// <summary>
    /// 是decimal类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDecimal(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsDecimal(predefined);
    /// <summary>
    /// 是decimal类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDecimal(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.DecimalKeyword);
    /// <summary>
    /// 是string类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsString(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsString(predefined);
    /// <summary>
    /// 是string类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsString(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.StringKeyword);
    /// <summary>
    /// 是char类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsChar(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsChar(predefined);
    /// <summary>
    /// 是char类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsChar(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.CharKeyword);
    /// <summary>
    /// 是Object类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObject(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsObject(predefined);
    /// <summary>
    /// 是Object类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObject(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ObjectKeyword);
    /// <summary>
    /// 是Void类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsVoid(predefined);
    /// <summary>
    /// 是Void类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.VoidKeyword);
    /// <summary>
    /// 空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax Nullable(this TypeSyntax type)
        => SyntaxFactory.NullableType(type);
    /// <summary>
    /// 数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayTypeSyntax Array(this TypeSyntax type, int rank = 1)
        => SyntaxFactory.ArrayType(type, SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(CheckArraySize(rank))));
    #region New
    /// <summary>
    /// 初始化数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCreationExpressionSyntax New(this ArrayTypeSyntax type, params ExpressionSyntax[] elements)
        => SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, SyntaxFactory.SeparatedList(elements)));
    /// <summary>
    /// 初始化数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCreationExpressionSyntax NewArray(this TypeSyntax type, params ExpressionSyntax[] elements)
        => New(Array(type, 1), elements);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, params IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), default);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<ExpressionSyntax> arguments)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), default);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<SyntaxToken> arguments)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), default);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<string> arguments)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), default);
    #endregion
    #region Throw
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, params IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, IEnumerable<ExpressionSyntax> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, IEnumerable<SyntaxToken> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, IEnumerable<string> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    #endregion
    /// <summary>
    /// 数组长度参数处理
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public static SeparatedSyntaxList<ExpressionSyntax> CheckArraySize(int rank = 1)
    {
        if (rank <= 0)
            return SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression());
        if (rank == 1)
            return SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression());
        return SyntaxFactory.SeparatedList<ExpressionSyntax>(Enumerable.Repeat(SyntaxFactory.OmittedArraySizeExpression(), rank));
    }
    /// <summary>
    /// 指针
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax Pointer(this TypeSyntax type)
        => SyntaxFactory.PointerType(type);
    #region Variable
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, SyntaxToken variableName)
        => SyntaxFactory.VariableDeclaration(type, SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(variableName)));
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, string variableName)
        => Variable(type, SyntaxFactory.Identifier(variableName));
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, SyntaxToken variableName, ExpressionSyntax value)
        => SyntaxFactory.VariableDeclaration(type, SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(variableName).WithInitializer(value)));
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, string variableName, ExpressionSyntax value)
        => Variable(type, SyntaxFactory.Identifier(variableName), value);
    #endregion
    #region Catch
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="catchType"></param>
    /// <param name="catchName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CatchDeclarationSyntax Catch(this TypeSyntax catchType, SyntaxToken catchName)
        => SyntaxFactory.CatchDeclaration(catchType, catchName);
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="catchType"></param>
    /// <param name="catchName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CatchDeclarationSyntax Catch(this TypeSyntax catchType, string catchName)
        => Catch(catchType, SyntaxFactory.Identifier(catchName));
    #endregion
    #region Field
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, SyntaxToken variableName)
        => SyntaxFactory.FieldDeclaration(Variable(type, variableName));
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, string variableName)
        => Field(type, SyntaxFactory.Identifier(variableName));
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, SyntaxToken variableName, ExpressionSyntax value)
        => SyntaxFactory.FieldDeclaration(Variable(type, variableName, value));
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, string variableName, ExpressionSyntax value)
        => Field(type, SyntaxFactory.Identifier(variableName), value);
    #endregion
    #region Property
    /// <summary>
    /// 定义属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, string propertyName, params AccessorDeclarationSyntax[] items)
        => Property(propertyType, SyntaxFactory.Identifier(propertyName), items);
    /// <summary>
    /// 定义属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, SyntaxToken propertyName, params AccessorDeclarationSyntax[] items)
        => SyntaxFactory.PropertyDeclaration(default, default, propertyType, default, propertyName, SyntaxFactory.AccessorList(SyntaxGenerator.List(items)), default, default, default);
    /// <summary>
    /// 定义自动属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="kinds">Get/Set/Init</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, string propertyName, params SyntaxKind[] kinds)
        => Property(propertyType, SyntaxFactory.Identifier(propertyName), kinds);
    /// <summary>
    /// 定义自动属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="kinds">Get/Set/Init</param>
    /// <returns></returns>
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, SyntaxToken propertyName, params SyntaxKind[] kinds)
    {
        var count = kinds.Length;
        var items = new AccessorDeclarationSyntax[count];
        for (int i = 0; i < count; i++)
        {
            // 自动属性(无代码)使用WithSemicolonToken增加;号结束
            items[i] = SyntaxFactory.AccessorDeclaration(kinds[i])
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        return Property(propertyType, propertyName, items);
    }
    #region PropertyGetOnly
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetOnlyProperty(this TypeSyntax propertyType, SyntaxToken propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration);
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    
    public static PropertyDeclarationSyntax GetOnlyProperty(this TypeSyntax propertyType, string propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration);
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetOnlyProperty(this TypeSyntax propertyType, SyntaxToken propertyName, ExpressionSyntax value)
        => Property(propertyType, propertyName, 
            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.GetKeyword), SyntaxFactory.Block(value.Return()), default, default));
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetOnlyProperty(this TypeSyntax propertyType, string propertyName, ExpressionSyntax value)
        => GetOnlyProperty(propertyType, SyntaxFactory.Identifier(propertyName), value);
    #endregion
    #endregion
    #region Parameter
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, SyntaxToken parameterName)
        => SyntaxFactory.Parameter(default, default, type, parameterName, null);
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, string parameterName)
        => Parameter(type, SyntaxFactory.Identifier(parameterName));
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, SyntaxToken parameterName, ExpressionSyntax defaultValue)
        => SyntaxFactory.Parameter(default, default, type, parameterName, SyntaxFactory.EqualsValueClause(defaultValue));
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, string parameterName, ExpressionSyntax value)
        => Parameter(type, SyntaxFactory.Identifier(parameterName), value);
    #endregion
    #region Method
    /// <summary>
    /// 定义方法
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="methodName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodDeclarationSyntax Method(this TypeSyntax returnType, string methodName, params ParameterSyntax[] parameters)
        => Method(returnType, SyntaxFactory.Identifier(methodName), parameters);
    /// <summary>
    /// 定义方法
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="methodName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodDeclarationSyntax Method(this TypeSyntax returnType, SyntaxToken methodName, params ParameterSyntax[] parameters)
        => SyntaxFactory.MethodDeclaration(returnType, methodName)
        .WithParameterList(ParameterList(parameters));
    #endregion
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax Constructor(this TypeDeclarationSyntax type, params ParameterSyntax[] parameters)
        => SyntaxGenerator.ConstructorDeclaration(type.Identifier, parameters);
    #region ParameterList
    /// <summary>
    /// 形参列表
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterListSyntax ParameterList(IEnumerable<ParameterSyntax> parameters)
        => SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters));
    #endregion
    /// <summary>
    /// 全局命名样式
    /// </summary>
    private static readonly SymbolDisplayFormat _globalStyle = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included‌);
    /// <summary>
    /// 自动包含包含样式(不包含全局命名空间)
    /// </summary>
    private static readonly SymbolDisplayFormat _containingStyle = SymbolDisplayFormat.FullyQualifiedFormat
      .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining);
    /// <summary>
    /// 全局命名
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax ToGlobalName(this ITypeSymbol symbol)
        => SyntaxFactory.ParseTypeName(symbol.ToDisplayString(_globalStyle));
    /// <summary>
    /// 自动包含命名
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToContainingName(this ITypeSymbol symbol)
        => SyntaxFactory.IdentifierName(symbol.ToDisplayString(_containingStyle));
    #region IsNullable
    /// <summary>
    /// 是否可空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullable(this INamedTypeSymbol type)
        => IsGenericType(type, SpecialType.System_Nullable_T);
    #endregion
    /// <summary>
    /// 是否泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool IsGenericType(this INamedTypeSymbol type, SpecialType genericType)
        => type.IsGenericType && type.ConstructedFrom.SpecialType == genericType;
    /// <summary>
    /// 判断是否包含泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool HasGenericType(this INamedTypeSymbol type, SpecialType genericType)
    {
        if (IsGenericType(type, genericType))
            return true;
        foreach (var subType in type.Interfaces)
        {
            if (IsGenericType(subType, genericType))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 获取泛型闭合接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetGenericCloseInterfaces(this INamedTypeSymbol type, SpecialType genericType)
    {
        if (IsGenericType(type, genericType))
        {
            yield return type;
            yield break;
        }
        var interfaces = type.Interfaces;
        foreach (var item in interfaces)
        {
            if (IsGenericType(item, genericType))
                yield return item;
        }
    }
    /// <summary>
    /// 确认Nullable类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static NullableTypeSyntax CheckNullable(this TypeSyntax type)
    {
        if (type is NullableTypeSyntax nullable)
            return nullable;
        return SyntaxFactory.NullableType(type);
    }
    /// <summary>
    /// 类型符号转化为类型语法
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeSyntax ToSyntax(this INamedTypeSymbol symbol)
    {
        if(IsNullable(symbol))
        {
            var original = symbol.TypeArguments[0];
            if(original is INamedTypeSymbol namedOriginal)
                return SyntaxFactory.NullableType(ToSyntax(namedOriginal));
            return SyntaxFactory.NullableType(ToGlobalName(original));
        }
        return symbol.SpecialType switch
        {
            SpecialType.System_Boolean => SyntaxGenerator.BoolType,
            SpecialType.System_Byte => SyntaxGenerator.ByteType,
            SpecialType.System_SByte => SyntaxGenerator.SByteType,
            SpecialType.System_Int16 => SyntaxGenerator.ShortType,
            SpecialType.System_UInt16 => SyntaxGenerator.UShortType,
            SpecialType.System_Int32 => SyntaxGenerator.IntType,
            SpecialType.System_UInt32 => SyntaxGenerator.UIntType,
            SpecialType.System_Int64 => SyntaxGenerator.LongType,
            SpecialType.System_UInt64 => SyntaxGenerator.ULongType,
            SpecialType.System_Single => SyntaxGenerator.FloatType,
            SpecialType.System_Double => SyntaxGenerator.DoubleType,
            SpecialType.System_Decimal => SyntaxGenerator.DecimalType,
            SpecialType.System_String => SyntaxGenerator.StringType,
            SpecialType.System_Char => SyntaxGenerator.CharType,
            SpecialType.System_Object => SyntaxGenerator.ObjectType,
            SpecialType.System_Void => SyntaxGenerator.VoidType,
            SpecialType.System_DateTime => SyntaxGenerator.DateTimeType,
            SpecialType.System_IDisposable => SyntaxGenerator.IDisposableType,
            _ => ToGlobalName(symbol)
        };
    }
}
