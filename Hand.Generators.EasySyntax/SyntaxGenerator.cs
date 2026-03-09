using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand;

/// <summary>
/// 类语法树构造器
/// </summary>
/// <param name="usings">引用</param>
/// <param name="type">类</param>
/// <param name="members">类成员</param>
public class SyntaxGenerator(List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, List<MemberDeclarationSyntax> members)
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
    /// 参数
    /// </summary>
    protected readonly List<ParameterSyntax> _parameters = [];
    /// <summary>
    /// 成员
    /// </summary>
    protected readonly List<MemberDeclarationSyntax> _members = members;
    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    /// <summary>
    /// 成员
    /// </summary>
    public List<MemberDeclarationSyntax> Members
        => _members;
    #endregion
    #region Using
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="usings"></param>
    public void Using(params UsingDirectiveSyntax[] usings)
    {
        var delta = Plus(_usings, usings);
        if (delta.Count == 0)
            return;
        _usings.AddRange(delta);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="names"></param>
    public void Using(params NameSyntax[] names)
    {
        int count = names.Length;
        if (count == 0)
            return;
        var list = new UsingDirectiveSyntax[count];
        for (int i = 0; i < count; i++)
            list[i] = SyntaxFactory.UsingDirective(names[i]);
        Using(list);
    }
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="parameter"></param>
    public void AddParameter(ParameterSyntax parameter)
        => _parameters.Add(parameter);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="member"></param>
    public void AddMember(MemberDeclarationSyntax member)
        => _members.Add(member);
    /// <summary>
    /// 增加成员
    /// </summary>
    /// <param name="members"></param>
    public void AddMembers(params MemberDeclarationSyntax[] members)
        => _members.AddRange(members);
    #endregion
    #region Declare
    /// <summary>
    /// 声明命名空间
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static NamespaceDeclarationSyntax NamespaceDeclaration(string name)
        => SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(name));
    /// <summary>
    /// 声明文件作用域命名空间
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static FileScopedNamespaceDeclarationSyntax FileScopedNamespaceDeclaration(string name)
        => SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName(name));
    #region RecordDeclaration
    /// <summary>
    /// 定义记录类型
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    public static RecordDeclarationSyntax RecordDeclaration(SyntaxToken recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName);
    /// <summary>
    /// 定义记录类型
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    public static RecordDeclarationSyntax RecordDeclaration(string recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName);
    #endregion
    #region RecordStructDeclaration
    /// <summary>
    /// 定义记录结构体
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    public static RecordDeclarationSyntax RecordStructDeclaration(SyntaxToken recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxKind.RecordStructDeclaration, SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName)
            .WithClassOrStructKeyword(SyntaxFactory.Token(SyntaxKind.StructKeyword));
    /// <summary>
    /// 定义记录结构体
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
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
    public static ConstructorDeclarationSyntax ConstructorDeclaration(string typeName, params ParameterSyntax[] parameters)
        => ConstructorDeclaration(SyntaxFactory.Identifier(typeName), parameters);
    /// <summary>
    /// 定义构造函数
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxToken typeName, params ParameterSyntax[] parameters)
        => SyntaxFactory.ConstructorDeclaration(typeName)
        .WithParameterList(ParameterList(parameters));
    #endregion
    /// <summary>
    /// 
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="returnType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax OperatorDeclaration(SyntaxKind kind, TypeSyntax returnType, params ParameterSyntax[] parameters)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(kind))
            .WithParameterList(ParameterList(parameters));
    }
    /// <summary>
    /// 含a、b参数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax EqualOperatorDeclaration(TypeSyntax type)
        => EqualOperatorDeclaration(type.Parameter("a"), type.Parameter("b"));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax EqualOperatorDeclaration(ParameterSyntax a, ParameterSyntax b)
        => SyntaxFactory.OperatorDeclaration(BoolType, SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken))
            .WithParameterList(ParameterList(a, b));
    /// <summary>
    /// 含a、b参数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax NotEqualOperatorDeclaration(TypeSyntax type)
        => NotEqualOperatorDeclaration(type.Parameter("a"), type.Parameter("b"));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax NotEqualOperatorDeclaration(ParameterSyntax a, ParameterSyntax b)
        => SyntaxFactory.OperatorDeclaration(BoolType, SyntaxFactory.Token(SyntaxKind.NotEqualsExpression))
            .WithParameterList(ParameterList(a, b));
    #region DeclareAccessor
    ///// <summary>
    ///// 定义处理器
    ///// </summary>
    ///// <param name="kind"></param>
    ///// <returns></returns>
    //public static AccessorDeclarationSyntax AccessorDeclaration(SyntaxKind kind)
    //    => SyntaxFactory.AccessorDeclaration(kind);
    /// <summary>
    /// 属性Get处理器
    /// </summary>
    /// <returns></returns>
    public static AccessorDeclarationSyntax PropertyGetDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
    /// <summary>
    /// 属性Get处理器
    /// </summary>
    /// <returns></returns>
    public static AccessorDeclarationSyntax PropertySetDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
    /// <summary>
    /// 属性Init处理器
    /// </summary>
    /// <returns></returns>
    public static AccessorDeclarationSyntax PropertyInitDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration);
    #endregion
    /// <summary>
    /// 参数列表
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
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
    #endregion
    #region Generic
    /// <summary>
    /// 泛型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="argumentTypes"></param>
    /// <returns></returns>
    public static GenericNameSyntax Generic(SyntaxToken name, params TypeSyntax[] argumentTypes)
        => SyntaxFactory.GenericName(name, SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(argumentTypes)));
    /// <summary>
    /// 泛型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="argumentTypes"></param>
    /// <returns></returns>
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
    /// int字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(int value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// uint字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(uint value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// long字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(long value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// ulong字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(ulong value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// float字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(float value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// double字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(double value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// decimal字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(decimal value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// string字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(string value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// char字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LiteralExpressionSyntax Literal(char value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal(value));
    #endregion
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
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
        var other = SyntaxFactory.Identifier("other");
        // obj is T other
        var checkType = parameter.ToIdentifierName().IsType(type, other);
        // Equals(other)
        var checkEquals = methodName.ToIdentifierName().Invocation([other]);
        // public override bool Equals(object obj) =>
        //     obj is T other && Equals(other);
        return BoolType.Method(methodName, parameter)
            .Public()
            .Override()
            .ToBuilder()
            .Return(checkType.And(checkEquals));
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
    public static CompilationUnitSyntax BuildUnit(List<UsingDirectiveSyntax> usings, MemberDeclarationSyntax root)
    {
        return SyntaxFactory.CompilationUnit()
            .WithUsings(List(usings))
            .AddMembers(root.WithLeadingTrivia(SyntaxFactory.TriviaList(
                SyntaxFactory.Comment("// <auto-generated/>")
            )))
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
            return type.AddParameterListParameters(parameters).AddMembers(members);
        return type.AddMembers(members);
    }
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="parameters"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static CompilationUnitSyntax Build(List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, ParameterSyntax[] parameters, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, CheckType(type, parameters, members));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="usings"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static CompilationUnitSyntax Build(BaseNamespaceDeclarationSyntax ns, List<UsingDirectiveSyntax> usings, TypeDeclarationSyntax type, ParameterSyntax[] parameters, MemberDeclarationSyntax[] members)
        => BuildUnit(usings, ns.AddMembers(CheckType(type, parameters, members)));
    /// <summary>
    /// 构造语法树
    /// </summary>
    /// <returns></returns>
    public virtual CompilationUnitSyntax Build()
        => Build(_usings, _type, [.. _parameters], [.. _members]);
    #endregion
    /// <summary>
    /// 复制类生成构造器
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SyntaxGenerator Clone(TypeDeclarationSyntax type)
    {
        var typeNew = SyntaxFactory.TypeDeclaration(type.Kind(), type.Identifier)
            .WithTypeParameterList(type.TypeParameterList)
            .WithKeyword(type.Keyword)
            .Partial();
        var parent = type.Parent;
        if (parent is null)
            return new SyntaxGenerator([], typeNew, []);
        else if(parent is BaseNamespaceDeclarationSyntax ns)
            // 清空成员并注释
            return new NamespaceBuilder(ns.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>()).WithLeadingTrivia(SyntaxFactory.TriviaList()), [], typeNew, []);
        else if (parent is CompilationUnitSyntax cu)
            return new SyntaxGenerator([.. cu.Usings], typeNew, []);
        return new SyntaxGenerator([], typeNew, []); 
    }
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static SyntaxGenerator Create(TypeDeclarationSyntax type, params MemberDeclarationSyntax[] members)
        => new([], type, [.. members]);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static NamespaceBuilder Create(BaseNamespaceDeclarationSyntax ns, TypeDeclarationSyntax type, params MemberDeclarationSyntax[] members)
        => new(ns, [], type, [.. members]);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static NamespaceBuilder Create(NameSyntax ns, TypeDeclarationSyntax type, params MemberDeclarationSyntax[] members)
        => new(SyntaxFactory.NamespaceDeclaration(ns), [], type, [.. members]);
    /// <summary>
    /// 生成构造器
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="type"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    public static NamespaceBuilder Create(string ns, TypeDeclarationSyntax type, params MemberDeclarationSyntax[] members)
        => new(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(ns)), [], type, [.. members]);
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public static SyntaxTree Using(SyntaxTree syntaxTree, params NameSyntax[] names)
    {
        int count = names.Length;
        if (count == 0)
            return syntaxTree;
        var list = new UsingDirectiveSyntax[count];
        for (int i = 0; i < count; i++)
            list[i] = SyntaxFactory.UsingDirective(names[i]);
        return Using(syntaxTree, list);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="syntaxTree"></param>
    /// <param name="usings"></param>
    /// <returns></returns>
    public static SyntaxTree Using(SyntaxTree syntaxTree, params UsingDirectiveSyntax[] usings)
    {
        var delta = Plus(syntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>(), usings);
        if (delta.Count == 0)
            return syntaxTree;
        var root = syntaxTree.GetCompilationUnitRoot()
            .AddUsings([.. delta]);
        return root.SyntaxTree;
    }
    /// <summary>
    /// 追加引用
    /// </summary>
    /// <param name="list0">原引用</param>
    /// <param name="delta">引用增量</param>
    /// <returns></returns>
    public static IReadOnlyCollection<UsingDirectiveSyntax> Plus(IEnumerable<UsingDirectiveSyntax> list0, UsingDirectiveSyntax[] delta)
    {
        var keys = new HashSet<string>(list0
            .Select(item => item.ToFullString())
            .Distinct());
        var list = new List<UsingDirectiveSyntax>(delta.Length);
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
}
