using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 类语法树构造器
/// </summary>
/// <param name="usings">引用</param>
/// <param name="type">类</param>
/// <param name="constructors">构造函数</param>
/// <param name="fields">字段</param>
/// <param name="properties">属性</param>
/// <param name="methods">方法</param>
public class SyntaxGenerator(List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
{
    #region 配置
    /// <summary>
    /// 引用
    /// </summary>
    protected readonly List<UsingDirectiveSyntax> _usings = usings;
    /// <summary>
    /// 类型
    /// </summary>
    protected readonly TypeDeclarationSyntax _type = type;
    /// <summary>
    /// 基类
    /// </summary>
    protected readonly List<BaseTypeSyntax> _baseTypes = [];
    /// <summary>
    /// 参数
    /// </summary>
    protected readonly List<ParameterSyntax> _parameters = [];
    /// <summary>
    /// 构造函数
    /// </summary>
    protected readonly List<ConstructorDeclarationSyntax> _constructors = constructors;
    /// <summary>
    /// 字段
    /// </summary>
    protected readonly List<FieldDeclarationSyntax> _fields = fields;
    /// <summary>
    /// 属性
    /// </summary>
    protected readonly List<PropertyDeclarationSyntax> _properties = properties;
    /// <summary>
    /// 方法
    /// </summary>
    protected readonly List<MethodDeclarationSyntax> _methods = methods;
    /// <summary>
    /// 成员
    /// </summary>
    protected readonly List<MemberDeclarationSyntax> _others = [];
    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    /// <summary>
    /// 参数
    /// </summary>
    public IEnumerable<ParameterSyntax> Parameters
        => _parameters;
    /// <summary>
    /// 成员
    /// </summary>
    public IEnumerable<MemberDeclarationSyntax> Members
        => _fields.Concat<MemberDeclarationSyntax>(_properties).Concat(_methods).Concat(_others);
    #endregion
    #region Using
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="usings"></param>
    public void Using(params IReadOnlyCollection<UsingDirectiveSyntax> usings)
    {
        if(usings.Count == 0)
            return;
        var delta = Plus(_usings, usings);
        if (delta.Count == 0)
            return;
        _usings.AddRange(delta);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="names"></param>
    public void Using(params IReadOnlyCollection<string> names)
    {
        if (names.Count == 0)
            return;
        var delta = Plus(_usings, names);
        if (delta.Count == 0)
            return;
        _usings.AddRange(delta);
    }
    /// <summary>
    /// 增加基类
    /// </summary>
    /// <param name="baseType"></param>
    public void AddBaseType(BaseTypeSyntax baseType)
        => _baseTypes.Add(baseType);
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="parameter"></param>
    public void AddParameter(ParameterSyntax parameter)
        => _parameters.Add(parameter);
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public void Parameter(TypeSyntax type, SyntaxToken name)
        => _parameters.Add(type.Parameter(name));
    /// <summary>
    /// 增加参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public void Parameter(TypeSyntax type, string name)
        => _parameters.Add(type.Parameter(name));
    /// <summary>
    /// 添加构造函数
    /// </summary>
    /// <param name="constructor"></param>
    public void AddConstructor(ConstructorDeclarationSyntax constructor)
        => _constructors.Add(constructor);
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="field"></param>
    public void AddField(FieldDeclarationSyntax field)
        => _fields.Add(field);
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="property"></param>
    public void AddProperty(PropertyDeclarationSyntax property)
        => _properties.Add(property);
    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="method"></param>
    public void AddMethod(MethodDeclarationSyntax method)
        => _methods.Add(method);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="member"></param>
    public void AddOther(MemberDeclarationSyntax member)
        => _others.Add(member);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="members"></param>
    public void AddOthers(params MemberDeclarationSyntax[] members)
        => _others.AddRange(members);
    #endregion
    #region Declare
    /// <summary>
    /// 声明命名空间
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceDeclarationSyntax NamespaceDeclaration(string name)
        => SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(name));
    /// <summary>
    /// 声明文件作用域命名空间
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileScopedNamespaceDeclarationSyntax FileScopedNamespaceDeclaration(string name)
        => SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName(name));
    #region RecordDeclaration
    /// <summary>
    /// 定义记录类型
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordDeclaration(SyntaxToken recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName);
    /// <summary>
    /// 定义记录类型
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordDeclaration(string recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName);
    #endregion
    #region RecordStructDeclaration
    /// <summary>
    /// 定义记录结构体
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordStructDeclaration(SyntaxToken recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxKind.RecordStructDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.RecordKeyword), SyntaxFactory.Token(SyntaxKind.StructKeyword), recordName, default, default, default, default, default, default, default, default);
    /// <summary>
    /// 定义记录结构体
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordStructDeclaration(string recordName)
        => RecordStructDeclaration(SyntaxFactory.Identifier(recordName));
    #endregion
    #region ConstructorDeclaration
    /// <summary>
    /// 定义构造函数
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax ConstructorDeclaration(string typeName, params ParameterSyntax[] parameters)
        => ConstructorDeclaration(SyntaxFactory.Identifier(typeName), parameters);
    /// <summary>
    /// 定义构造函数
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxToken typeName, params ParameterSyntax[] parameters)
        => SyntaxFactory.ConstructorDeclaration(default, default, typeName, ParameterList(parameters), default, default, default, default);
    #endregion
    #region PrimaryConstructorBaseType
    /// <summary>
    /// 主构造基类
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(TypeSyntax baseType, params ExpressionSyntax[] baseArguments)
        => SyntaxFactory.PrimaryConstructorBaseType(baseType, ArgumentList(baseArguments));
    /// <summary>
    /// 主构造基类
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(string baseType, params ExpressionSyntax[] baseArguments)
        => SyntaxFactory.PrimaryConstructorBaseType(SyntaxFactory.IdentifierName(baseType), ArgumentList(baseArguments));
    #endregion    
    #region OperatorDeclaration
    /// <summary>
    /// 运算符重载定义
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="returnType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax OperatorDeclaration(SyntaxKind kind, TypeSyntax returnType, params ParameterSyntax[] parameters)
        => SyntaxFactory.OperatorDeclaration(default, SyntaxFactory.TokenList(GenerateServices._public, GenerateServices._static), returnType, default, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), default, SyntaxFactory.Token(kind), ParameterList(parameters), default, default, default);
    /// <summary>
    /// 含a、b参数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax EqualOperatorDeclaration(TypeSyntax type)
        => EqualOperatorDeclaration(type.Parameter("a"), type.Parameter("b"));
    /// <summary>
    /// 重载==
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax EqualOperatorDeclaration(ParameterSyntax a, ParameterSyntax b)
        => OperatorDeclaration(SyntaxKind.EqualsEqualsToken, BoolType, a, b);
    /// <summary>
    /// 含a、b参数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax NotEqualOperatorDeclaration(TypeSyntax type)
        => NotEqualOperatorDeclaration(type.Parameter("a"), type.Parameter("b"));
    /// <summary>
    /// 重载!=
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax NotEqualOperatorDeclaration(ParameterSyntax a, ParameterSyntax b)
        => OperatorDeclaration(SyntaxKind.ExclamationEqualsToken, BoolType, a, b);
    #endregion
    #region DeclareAccessor
    /// <summary>
    /// 属性Get处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyGetDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
    /// <summary>
    /// 属性Get处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyGetDeclaration(ExpressionSyntax expression)
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.GetKeyword), default, ExpressionBody(expression), SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 属性Set处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertySetDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
    /// <summary>
    /// 属性Set处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertySetDeclaration(ExpressionSyntax expression)
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.SetKeyword), default, ExpressionBody(expression), SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 属性Init处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyInitDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration);
    /// <summary>
    /// 属性Init处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyInitDeclaration(ExpressionSyntax expression)
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.InitKeyword), default, ExpressionBody(expression), SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    #endregion
    /// <summary>
    /// 参数列表
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterListSyntax ParameterList(params IEnumerable<ParameterSyntax> parameters)
        => SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters));
    #endregion
    #region List
    /// <summary>
    /// 集合转化
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static SyntaxList<TItem> List<TItem>(TItem[] items)
        where TItem : SyntaxNode
    {
        return items.Length switch
        {
            0 => default,
            1 => new SyntaxList<TItem>(items[0]),
            _ => [.. items],
        };
    }
    /// <summary>
    /// 集合转化
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public static SyntaxList<TItem> List<TItem>(IList<TItem> items)
        where TItem : SyntaxNode
    {
        return items.Count switch
        {
            0 => default,
            1 => new SyntaxList<TItem>(items[0]),
            _ => [.. items],
        };
    }
    #endregion
    #region ArgumentList
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<ExpressionSyntax> arguments)
        => ArgumentList(arguments.Select(SyntaxFactory.Argument));
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<SyntaxToken> arguments)
        => ArgumentList(arguments.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    /// <summary>
    /// 实参列表
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentListSyntax ArgumentList(params IEnumerable<string> arguments)
        => ArgumentList(arguments.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    #endregion
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
            return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
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
    #region FrameworkMajorVersion
    private static readonly Lazy<int> _lazyFrameworkMajorVersion = new(GetFrameworkMajorVersion, true);
    /// <summary>
    /// .net主版本
    /// </summary>
    public static int FrameworkMajorVersion
        => _lazyFrameworkMajorVersion.Value;
    /// <summary>
    /// 获取.net主版本
    /// </summary>
    /// <returns></returns>
    private static int GetFrameworkMajorVersion()
    {
        var attribute = typeof(object).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute is null)
            return 0;
        var versionString = attribute.InformationalVersion;
        int index = versionString.IndexOf('.');
#if NET7_0_OR_GREATER
        if (index >= 0 && int.TryParse(versionString.AsSpan(0, index), out var majorVersion))
#else
        if (index >= 0 && int.TryParse(versionString.Substring(0, index), out var majorVersion))
#endif
            return majorVersion;
        return 0;
    }
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
    #region Literal
    /// <summary>
    /// null
    /// </summary>
    public static LiteralExpressionSyntax NullLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
    /// <summary>
    /// default
    /// </summary>
    public static LiteralExpressionSyntax DefaultLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
    /// <summary>
    /// true
    /// </summary>
    public static LiteralExpressionSyntax TrueLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
    /// <summary>
    /// false
    /// </summary>
    public static LiteralExpressionSyntax FalseLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
    /// <summary>
    /// this
    /// </summary>
    public static LiteralExpressionSyntax ThisLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.ThisExpression);
    /// <summary>
    /// bool字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(bool value)
        => value ? TrueLiteral : FalseLiteral;
    /// <summary>
    /// int字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(int value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// uint字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(uint value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// long字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(long value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// ulong字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(ulong value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// float字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(float value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// double字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(double value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// decimal字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(decimal value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// string字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(string value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// char字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(char value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// 基础类型转化为字面量表达式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static LiteralExpressionSyntax Literal(SpecialType type, object value)
    {        
        return type switch
        {
            SpecialType.System_Boolean => Literal((bool)value),
            SpecialType.System_Int16 => Literal((short)value),
            SpecialType.System_UInt16 => Literal((ushort)value),
            SpecialType.System_Int32 => Literal((int)value),
            SpecialType.System_UInt32 => Literal((uint)value),
            SpecialType.System_Int64 => Literal((long)value),
            SpecialType.System_UInt64 => Literal((ulong)value),
            SpecialType.System_String => Literal((string)value),
            SpecialType.System_Char => Literal((char)value),
            SpecialType.System_Decimal => Literal((decimal)value),
            SpecialType.System_Double => Literal((double)value),
            SpecialType.System_Single => Literal((float)value),
            _ => throw new ArgumentException("字面量类型不支持"),
        };
    }
    #endregion
    #region Collection
    /// <summary>
    /// 集合表达式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollectionExpressionSyntax Collection(params ExpressionSyntax[] items)
        => SyntaxFactory.CollectionExpression(SyntaxFactory.SeparatedList(Array.ConvertAll<ExpressionSyntax, CollectionElementSyntax>(items, static item => SyntaxFactory.ExpressionElement(item))));
    #endregion
    /// <summary>
    /// 元组表达式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TupleExpressionSyntax Tuple(params ExpressionSyntax[] items)
        => SyntaxFactory.TupleExpression(SyntaxFactory.SeparatedList(Array.ConvertAll(items, static item => SyntaxFactory.Argument(item))));
    /// <summary>
    /// Null模式
    /// </summary>
    public static ConstantPatternSyntax NullPattern
        => SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
    /// <summary>
    /// NotNull模式
    /// </summary>
    public static UnaryPatternSyntax NotNullPattern
        => SyntaxFactory.UnaryPattern(NullPattern);
    #region RelationalPatternSyntax
    #region GreaterThanPattern
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterThanPattern(int number)
        => GreaterThanPattern(Literal(number));
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterThanPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.GreaterThanToken), number);
    #endregion
    #region GreaterOrEqualPattern
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterOrEqualPattern(int number)
        => GreaterOrEqualPattern(Literal(number));
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterOrEqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.GreaterThanEqualsToken), number);
    #endregion
    #region LessThanPattern
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessThanPattern(int number)
        => LessThanPattern(Literal(number));
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessThanPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanToken), number);
    #endregion
    #region LessOrEqualPattern
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessOrEqualPattern(int number)
        => LessOrEqualPattern(Literal(number));
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessOrEqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanEqualsToken), number);
    #endregion
    #region EqualPattern
    /// <summary>
    /// !=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax EqualPattern(int number)
        => EqualPattern(Literal(number));
    /// <summary>
    /// ==
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax EqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken), number);
    #endregion
    #region NotEqualPattern
    /// <summary>
    /// ==
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax NotEqualPattern(int number)
        => NotEqualPattern(Literal(number));
    /// <summary>
    /// !=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax NotEqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken), number);
    #endregion
    #endregion
    #region VarPattern
    /// <summary>
    /// var模式
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern()
        => SyntaxFactory.VarPattern(SyntaxFactory.DiscardDesignation());
    /// <summary>
    /// var模式
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(SyntaxToken variableName)
        => SyntaxFactory.VarPattern(SyntaxFactory.SingleVariableDesignation(variableName));
    /// <summary>
    /// var模式
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(string variableName)
        => SyntaxFactory.VarPattern(SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(variableName)));
    #endregion
    #region VarParenthesizedPattern
    /// <summary>
    /// var括号模式
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarParenthesizedPattern(params IEnumerable<SingleVariableDesignationSyntax> variables)
        => SyntaxFactory.VarPattern(SyntaxFactory.ParenthesizedVariableDesignation(SyntaxFactory.SeparatedList<VariableDesignationSyntax>(variables)));
    /// <summary>
    /// var括号模式
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarParenthesizedPattern(params IEnumerable<SyntaxToken> variables)
        => VarParenthesizedPattern(variables.Select(static name => SyntaxFactory.SingleVariableDesignation(name)));
    /// <summary>
    /// var括号模式
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarParenthesizedPattern(params IEnumerable<string> variables)
        => VarParenthesizedPattern(variables.Select(static name => SyntaxFactory.Identifier(name)));
    #endregion
    /// <summary>
    /// or模式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static PatternSyntax OrPattern(params ExpressionSyntax[] items)
    {
        var count = items.Length;
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(items));
        PatternSyntax pattern = SyntaxFactory.ConstantPattern(items[0]);
        for (var i = 1; i < count; i++)
            pattern = pattern.Or(SyntaxFactory.ConstantPattern(items[i]));
        return pattern;
    }
    /// <summary>
    /// and模式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static PatternSyntax AndPattern(params ExpressionSyntax[] items)
    {
        var count = items.Length;
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(items));
        PatternSyntax pattern = SyntaxFactory.ConstantPattern(items[0]);
        for (var i = 1; i < count; i++)
            pattern = pattern.And(SyntaxFactory.ConstantPattern(items[i]));
        return pattern;
    }    
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InitializerExpressionSyntax Initializer(params IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>(items));
    /// <summary>
    /// 表达式方法体
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrowExpressionClauseSyntax ExpressionBody(ExpressionSyntax expression)
        => SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), expression);
    #region New
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<ArgumentSyntax> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<ExpressionSyntax> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<SyntaxToken> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<SyntaxToken> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<string> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<string> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(), Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<ExpressionSyntax> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), Initializer(items));
    #endregion
    #region Interpolation
    /// <summary>
    /// 开始构造插值表达式
    /// </summary>
    /// <param name="start">String/VerbatimString/SingleLineRawString/MultiLineRawString</param>
    /// <param name="end">String/MultiLineRawString</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InterpolationBuilder Interpolation(SyntaxKind start, SyntaxKind end)
        => new(start, end);
    /// <summary>
    /// 开始构造插值表达式
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InterpolationBuilder Interpolation()
        => new(SyntaxKind.InterpolatedStringStartToken, SyntaxKind.InterpolatedStringEndToken);
    #endregion
    /// <summary>
    /// try
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TryBuilder Try()
        => new();
    /// <summary>
    /// Scope
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScopeBuilder Scope()
        => new([]);
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(string message)
        => SyntaxFactory.IdentifierName("Exception")
        .Throw([Literal(message)]);
    /// <summary>
    /// 重写Equals
    /// </summary>
    /// <param name="type"></param>
    public static MethodDeclarationSyntax ObjectEqualsDeclaration(TypeSyntax type)
    {
        // (object obj)
        var parameter = ObjectType.Parameter("obj");
        var methodName = SyntaxFactory.Identifier(nameof(Equals));
        var other = SyntaxFactory.IdentifierName("other");
        // obj is T other
        var checkType = parameter.ToIdentifierName().IsType(type, other.Identifier);
        // Equals(other)
        var checkEquals = methodName.ToIdentifierName().Invocation([other]);
        // public override bool Equals(object obj) =>
        //     obj is T other && Equals(other);
        return BoolType.Method(methodName, parameter)
            .Public()
            .Override()
            .ToBuilder()
            .Return(checkType.LogicalAnd(checkEquals));
    }
    /// <summary>
    /// 生成判等运算符重载
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nullCondition"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax BuildEqualOperator(TypeSyntax type, bool nullCondition)
    {
        var a = SyntaxFactory.IdentifierName("a");
        var b = SyntaxFactory.IdentifierName("b");
        // public static bool operator ==(T a, T b)")
        var builder = EqualOperatorDeclaration(type)
            .ToBuilder();
        if (nullCondition)
        {
            return builder
                // {
                .Block()
                // if(a is null) return false;
                .If(a.IsNull()).ReturnFalse()
                // return a.Equals(b);
                .Return(a.Access("Equals").Invocation([b]))
                .End();
        }
        else
        {
            // return a.Equals(b);
            return builder.Return(a.Access("Equals").Invocation([b]));
        }
    }
    /// <summary>
    /// 生成判等运算符重载
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nullCondition"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax BuildNotEqualOperator(TypeSyntax type, bool nullCondition)
    {
        var a = SyntaxFactory.IdentifierName("a");
        var b = SyntaxFactory.IdentifierName("b");
        // public static bool operator !=(T a, T b)")
        var builder = NotEqualOperatorDeclaration(type)
            .ToBuilder();
        if (nullCondition)
        {
            return builder
                // {
                .Block()
                // if(a is null) return true;
                .If(a.IsNull()).ReturnTrue()
                // return !a.Equals(b);
                .Return(a.Access("Equals").Invocation([b]).LogicalNot())
                .End();
        }
        else
        {
            // return !a.Equals(b);
            return builder.Return(a.Access("Equals").Invocation([b]).LogicalNot());
        }

    }
    #region Build
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="usings"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax BuildUnit(List<UsingDirectiveSyntax> usings, MemberDeclarationSyntax root)
    {
        return SyntaxFactory.CompilationUnit()
            .WithUsings(List(usings))
            .AddMembers(root)
            .NormalizeWhitespace();
    }
    /// <summary>
    /// 处理类型声明
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameters"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static TypeDeclarationSyntax CheckType(TypeDeclarationSyntax type, ParameterSyntax[] parameters, MemberDeclarationSyntax[] members)
    {
        if (parameters.Length > 0)
            return CheckMembers(type.AddParameterListParameters(parameters), members);
        return CheckMembers(type, members);
    }
    /// <summary>
    /// 处理基类
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseTypes"></param>
    /// <returns></returns>
    public static TypeDeclarationSyntax CheckBaseTypes(TypeDeclarationSyntax type, BaseTypeSyntax[] baseTypes)
    {
        return baseTypes.Length == 0 ? type:(TypeDeclarationSyntax)type.AddBaseListTypes(baseTypes);
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    private static TypeDeclarationSyntax CheckMembers(TypeDeclarationSyntax type,  MemberDeclarationSyntax[] members)
    {
        if (members.Length > 0)
        {
            if (type.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
            {
                // 如果简化类型(分号结尾)
                // 增加花括号并去掉分号
                return type.AddMembers(members)
                    .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
            }
            else
            {
                return type.AddMembers(members);
            }
        }
        if(type.Members.Count == 0 && !type.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
        {
            // 如果没有成员且没有分号结尾
            // 增加分号结尾
            return type.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        return type;
    }
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="baseTypes"></param>
    /// <param name="parameters"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax Build(List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, BaseTypeSyntax[] baseTypes, ParameterSyntax[] parameters, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, CheckType(CheckBaseTypes(type, baseTypes), parameters, members));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="baseTypes"></param>
    /// <param name="parameters"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax Build(BaseNamespaceDeclarationSyntax ns, List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, BaseTypeSyntax[] baseTypes, ParameterSyntax[] parameters, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, ns.AddMembers(CheckType(CheckBaseTypes(type, baseTypes), parameters, members)));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual CompilationUnitSyntax Build()
        => Build(_usings, _type, [.. _baseTypes], [.. _parameters], [.. _constructors, .. _fields, .. _properties, .. _methods, .. _others]);
    #endregion
    /// <summary>
    /// 复制类生成构造器
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SyntaxGenerator Clone(TypeDeclarationSyntax type)
    {
        var typeNew = SyntaxFactory.TypeDeclaration(
            type.Kind(),
            default,
            new SyntaxTokenList(GenerateServices._partial),
            type.Keyword,
            type.Identifier,
            typeParameterList: type.TypeParameterList,
            baseList: null,
            type.ConstraintClauses,
            type.OpenBraceToken,
            default,
            type.CloseBraceToken,
            type.SemicolonToken);

        var parent = type.Parent;
        if (parent is null)
            return new SyntaxGenerator([], typeNew, [], [], [], []);
        else if(parent is BaseNamespaceDeclarationSyntax ns)
            // 清空成员并注释
            return new NamespaceBuilder(ns.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>()).WithLeadingTrivia(SyntaxFactory.TriviaList()), [], typeNew, [], [], [], []);
        else if (parent is CompilationUnitSyntax cu)
            return new SyntaxGenerator([.. cu.Usings], typeNew, [], [], [], []);
        return new SyntaxGenerator([], typeNew, [], [], [], []); 
    }
    #region Create
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type">类</param>
    /// <param name="constructors">构造函数</param>
    /// <param name="fields">字段</param>
    /// <param name="properties">属性</param>
    /// <param name="methods">方法</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new([], type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SyntaxGenerator Create(TypeDeclarationSyntax type)
        => new([], type, [], [], [], []);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <param name="constructors">构造函数</param>
    /// <param name="fields">字段</param>
    /// <param name="properties">属性</param>
    /// <param name="methods">方法</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(ns, [], type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type)
        => new(ns, [], type, [], [], [], []);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <param name="constructors">构造函数</param>
    /// <param name="fields">字段</param>
    /// <param name="properties">属性</param>
    /// <param name="methods">方法</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(string ns, TypeDeclarationSyntax type, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), [], type, constructors, fields, properties, methods);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type">类</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceBuilder Create(string ns, TypeDeclarationSyntax type)
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), [], type, [], [], [], []);
    #endregion
    /// <summary>
    /// 追加引用
    /// </summary>
    /// <param name="list0">原引用</param>
    /// <param name="delta">引用增量</param>
    /// <returns></returns>
    public static IReadOnlyCollection<UsingDirectiveSyntax> Plus(IEnumerable<UsingDirectiveSyntax> list0, params IReadOnlyCollection<UsingDirectiveSyntax> delta)
    {
        var keys = new HashSet<string>(list0
            .Select(item => item.ToFullString())
            .Distinct());
        var list = new List<UsingDirectiveSyntax>(delta.Count);
        foreach (var item in delta)
        {
            var key = item.ToFullString();
            if (keys.Contains(key))
                continue;
            keys.Add(key);
            list.Add(item);
        }
        return list;
    }
    /// <summary>
    /// 追加引用
    /// </summary>
    /// <param name="list0">原引用</param>
    /// <param name="delta">引用增量</param>
    /// <returns></returns>
    public static IReadOnlyCollection<UsingDirectiveSyntax> Plus(IEnumerable<UsingDirectiveSyntax> list0, params IReadOnlyCollection<string> delta)
    {
        var keys = new HashSet<string>(list0
            .Select(item => item.ToFullString())
            .Distinct());
        var list = new List<UsingDirectiveSyntax>(delta.Count);
        foreach (var item in delta)
        {
            if (keys.Contains(item))
                continue;
            keys.Add(item);
            list.Add(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(item)));
        }
        return list;
    }
}
