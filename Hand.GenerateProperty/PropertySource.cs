using Hand.Builders;
using Hand.Naming;
using Hand.Reflection;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.GenerateProperty;

/// <summary>
/// 实体属性源对象
/// </summary>
/// <param name="type"></param>
/// <param name="compilation"></param>
/// <param name="symbol"></param>
/// <param name="originalSymbol"></param>
/// <param name="rule"></param>
public class PropertySource(TypeDeclarationSyntax type, Compilation compilation, INamedTypeSymbol symbol, INamedTypeSymbol originalSymbol, PropertyRule rule)
    : IGeneratorSource
{
    #region 配置
    private readonly CompositeProvider _descriptor = CompositeProvider.DeclaredOnly(symbol);
    private readonly TypeDeclarationSyntax _type = type;
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol _symbol = symbol;
    private readonly bool _nullable = symbol.IsNullable();
    private readonly INamedTypeSymbol _originalSymbol = originalSymbol;
    private readonly TypeSyntax _originalType = originalSymbol.ToSyntax();
    private readonly bool _originalNullable = originalSymbol.IsNullable();
    private readonly PropertyRule _rule = rule;
    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type 
        => _type;
    /// <summary>
    /// 反射信息
    /// </summary>
    public INamedTypeSymbol Symbol 
        => _symbol;
    /// <summary>
    /// 规则
    /// </summary>
    public PropertyRule Rule 
        => _rule;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_symbol.ToDisplayString()}.Property.g.cs";
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        IdentifierNameSyntax member = BuildOriginal(builder);
        if (_rule.ToStringMethod)
            builder.AddMethod(BuildToString(member, _originalType, _originalNullable));
        if (_rule.GetHashCodeMethod)
            builder.AddMethod(BuildGetHashCode(member, _originalNullable));
        return builder;
    }
    /// <summary>
    /// 定义Original
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IdentifierNameSyntax BuildOriginal(SyntaxGenerator builder)
    {
        IdentifierNameSyntax member;
        if (_symbol.IsRecord && !_descriptor.HasConstructor())
        {
            member = SyntaxFactory.IdentifierName("Original");
            builder.AddParameter(_originalType.Parameter(member.Identifier));
            return member;
        }
        member = BuildProperty(builder);
        if (_rule.Constructor && !_descriptor.ConstructorContains(_originalSymbol))
        {
            var original = SyntaxFactory.IdentifierName("original");
            var constructor = _type.Constructor(_originalType.Parameter(original.Identifier))
                .Public()
                .ToBuilder()
                .AddExpression(member.Assign(original))
                .End();
            builder.AddConstructor(constructor);
        }
        if (!_symbol.IsRecord)
            BuildEqualOperator(builder, member);
        return member;
    }
    /// <summary>
    /// 定义属性(及字段)
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IdentifierNameSyntax BuildProperty(SyntaxGenerator builder)
    {
        const string propertyName = "Original";
        var Original = SyntaxFactory.IdentifierName(propertyName);
        // 属性Original已存在,忽略
        if (_descriptor.Contains(propertyName))
            return Original;
        if (_rule.Field)
        {
            var accessorList = new List<AccessorDeclarationSyntax>();
            var _original = SyntaxFactory.IdentifierName("_original");
            var field = _originalType.Field(_original.Identifier)
                .Private();
            // get => _original;
            accessorList.Add(SyntaxGenerator.PropertyGetDeclaration()
                .ToBuilder()
                .Return(_original));
            if (_symbol.IsReadOnly)
            {
                field = field.ReadOnly();
            }
            else
            {
                // set => _original = value;
                accessorList.Add(SyntaxGenerator.PropertySetDeclaration()
                        .ToBuilder()
                        .AddExpression(_original.AssignValue())
                        .End());
            }
            var property = _originalType.Property(Original.Identifier, accessorList.ToArray())
                .Public();
            builder.AddField(field);
            builder.AddProperty(property);
            return _original;
        }
        else
        {
            var accessorList = new List<SyntaxKind>() { SyntaxKind.GetAccessorDeclaration };
            if (!_rule.Constructor)
            {
                // 无构造函数时,属性需要支持init
                accessorList.Add(SyntaxKind.InitAccessorDeclaration);
            }
            var property = _originalType.Property(Original.Identifier, accessorList.ToArray())
                .Public();
            builder.AddProperty(property);
            return Original;
        }
    }
    /// <summary>
    /// 生成Equals和重载运算符
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="original"></param>
    public void BuildEqualOperator(SyntaxGenerator builder, IdentifierNameSyntax original)
    {
        var type = SyntaxFactory.ParseTypeName(_symbol.Name);
        var hasEquals = _descriptor.MethodContains("Equals", _symbol);
        // 定义 Equals
        if (_rule.EqualsMethod)
        {
            if (!hasEquals)
                builder.AddMethod(BuildEquals(type, _nullable, original, _originalNullable));
            hasEquals = true;
            if (!_descriptor.MethodContains("Equals", [_compilation.GetObjectSymbol()]))
                builder.AddMethod(SyntaxGenerator.ObjectEqualsDeclaration(type));
        }
        // 重载需要调用Equals
        if (hasEquals && _rule.Operator)
        {
            builder.AddOthers(
                SyntaxGenerator.BuildEqualOperator(type, _nullable),
                SyntaxGenerator.BuildNotEqualOperator(type, _nullable));
        }
    }
    #region 静态方法
    /// <summary>
    /// 生成ToString
    /// </summary>
    /// <param name="original"></param>
    /// <param name="type"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static MethodDeclarationSyntax BuildToString(IdentifierNameSyntax original, TypeSyntax type, bool nullable)
    {
        return SyntaxGenerator.StringType.Method("ToString")
            .Public()
            .Override()
            .ToBuilder()
            .Return(CheckToString(original, type, nullable));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nullable"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public static ExpressionSyntax CheckToString(IdentifierNameSyntax original, TypeSyntax type, bool nullable)
    {
        if (type.IsString())
            return original;
        if (nullable)
            return SyntaxFactory.InvocationExpression(original.ConditionalAccess("ToString"));
        return SyntaxFactory.InvocationExpression(original.Access("ToString"));
    }
    /// <summary>
    /// 生成GetHashCode
    /// </summary>
    /// <param name="original"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static MethodDeclarationSyntax BuildGetHashCode(IdentifierNameSyntax original, bool nullable)
    {
        return SyntaxGenerator.IntType.Method("GetHashCode")
            .Public()
            .Override()
            .ToBuilder()
            .Return(CheckGetHashCode(original, nullable));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static ExpressionSyntax CheckGetHashCode(IdentifierNameSyntax original, bool nullable)
    {
        var expression = original.Access("GetHashCode").Invocation();
        if (nullable)
            return SyntaxFactory.ConditionalExpression(original.IsNull(), SyntaxGenerator.Literal(0), expression);
        return expression;
    }
    /// <summary>
    /// 生成Equals
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nullable"></param>
    /// <param name="original"></param>
    /// <param name="originalNullCondition"></param>
    /// <returns></returns>
    public static MethodDeclarationSyntax BuildEquals(TypeSyntax type, bool nullable, IdentifierNameSyntax original, bool originalNullCondition)
    {
        var parameter = type.Parameter("other");
        var method = SyntaxGenerator.BoolType.Method("Equals", parameter)
            .Public();        
        StatementBuilder<MethodDeclarationSyntax> methodBuilder = method.ToBuilder();
        var other = parameter.ToIdentifierName();
        if (nullable)
        {
            var @this = SyntaxFactory.IdentifierName("this");
            var referenceEquals = SyntaxFactory.IdentifierName(nameof(ReferenceEquals));

            methodBuilder = methodBuilder
                // if(other is null)
                .If(other.IsNull())
                // return false;
                .ReturnFalse()
                // if(ReferenceEquals(this, other))
                .If(referenceEquals.Invocation([@this, other]))
                // return true;
                .ReturnTrue();
        }
        if (originalNullCondition)
        {
            // var otherOriginal = other.Original
            var otherOriginal = other.Access(original);
            //var otherOriginal = SyntaxFactory.VariableDeclarator("otherOriginal")
            //    .WithInitializer(other.Access(original));
            return methodBuilder
                //.Declare(otherOriginal)
                // if(Original is null)
                .If(original.IsNull())
                // {
                .Block()
                // if(otherOriginal is null)
                .If(otherOriginal.IsNull())
                // reurn true;
                .ReturnTrue()
                // reurn fase;
                .ReturnFalse()
                // }
                .End()
                // reurn Original.Equals(otherOriginal);
                .Return(original.Access("Equals").Invocation([otherOriginal]));
        }
        else
        {
            // return Original.Equals(other.Original);
            return methodBuilder.Return(original.Access("Equals").Invocation([other.Access(original)]));
        }
    }
    #endregion
}
