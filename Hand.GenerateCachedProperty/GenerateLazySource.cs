using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.GenerateCachedProperty;

/// <summary>
/// 构造延迟缓存源对象
/// </summary>
public abstract class GenerateLazySource(TypeDeclarationSyntax type, INamedTypeSymbol typeSymbol, string propertyName, INamedTypeSymbol propertySymbol, bool isStatic, string valueName, string stateName, string lockName)
    : IGeneratorSource
{
    /// <summary>
    /// 构造延迟缓存源对象
    /// </summary>
    public GenerateLazySource(TypeDeclarationSyntax type, INamedTypeSymbol typeSymbol, string propertyName, INamedTypeSymbol propertySymbol, bool isStatic)
        : this(type, typeSymbol, propertyName, propertySymbol, isStatic, "_value" + propertyName, "_state" + propertyName, "_lock" + propertyName)
    {
    }
    #region 配置
    private readonly TypeDeclarationSyntax _type = type;
    private readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    private readonly bool _isStatic = isStatic;
    private readonly string _propertyName = propertyName;
    private readonly string _valueName = valueName;
    private readonly string _stateName = stateName;
    private readonly string _lockName = lockName;
    private readonly IdentifierNameSyntax _value = SyntaxFactory.IdentifierName(valueName);
    private readonly IdentifierNameSyntax _state = SyntaxFactory.IdentifierName(stateName);
    private readonly IdentifierNameSyntax _lock = SyntaxFactory.IdentifierName(lockName);
    private readonly TypeSyntax _propertyType = SyntaxFactory.ParseTypeName(propertySymbol.ToDisplayString());
    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type 
        => _type;
    /// <summary>
    /// 反射信息
    /// </summary>
    public INamedTypeSymbol Symbol 
        => _typeSymbol;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_typeSymbol.ToDisplayString()}.GenerateLazy{_propertyName}.g.cs";
    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName 
        => _propertyName;
    /// <summary>
    /// 值对象名
    /// </summary>
    public string ValueName 
        => _valueName;
    /// <summary>
    /// 状态名
    /// </summary>
    public string StateName 
        => _stateName;
    /// <summary>
    /// 锁
    /// </summary>
    public string LockName 
        => _lockName;
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var _valueField = _propertyType.Field(_value.Identifier, SyntaxGenerator.DefaultLiteral)
            .Private();
        var _stateField = SyntaxGenerator.BoolType.Field(_state.Identifier, SyntaxGenerator.FalseLiteral)
            .Private();
        var _lockField = SyntaxGenerator.LockType.Field(_lock.Identifier, SyntaxFactory.ImplicitObjectCreationExpression())
            .Private();
        var property = _propertyType.Property(_propertyName, CreateAccessor())
            .Public();
        if (_isStatic)
        {
            builder.AddMember(_valueField.Static());
            builder.AddMember(_stateField.Static());
            builder.AddMember(_lockField.Static());
            builder.AddMember(property.Static());
        }
        else
        {
            builder.AddMember(_valueField);
            builder.AddMember(_stateField);
            builder.AddMember(_lockField);
            builder.AddMember(property);
        }
        
        return builder;
    }
    /// <summary>
    /// 构造属性处理器
    /// </summary>
    /// <returns></returns>
    public AccessorDeclarationSyntax CreateAccessor()
    {
        return SyntaxGenerator.PropertyGetDeclaration()
            .ToBuilder()
            // if(_state)
            .If(_state)
                // return _value
                .Return(_value)
            // lock(_lock){
            .Lock(_lock)
                //if(_state)
                .If(_state)
                    // return _value
                    .Return(_value)
                // _value = GetValue()
                .Add(_value.Assign(GetValueExpression()))
                // _state = true
                .Add(_state.Assign(SyntaxGenerator.TrueLiteral))
                // }
                .End()
            // reurn _value
            .Return(_value);
    }
    /// <summary>
    /// 获取值表达式
    /// </summary>
    /// <returns></returns>
    protected abstract ExpressionSyntax GetValueExpression();
    /// <summary>
    /// 检查属性名
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public static string CheckPropertyName(string? propertyName, SyntaxToken identifier)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return "Lazy" + identifier.ValueText;
        return propertyName!;
    }
    /// <summary>
    /// 是否不可变
    /// </summary>
    /// <param name="modifiers"></param>
    /// <returns></returns>
    public static bool CheckImMutable(SyntaxTokenList modifiers)
    {
        if (modifiers.Any(SyntaxKind.SealedKeyword))
            return true;
        if (modifiers.HasKinds(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
            return false;
        return true;
    }
    /// <summary>
    /// 获取返回表达式
    /// </summary>
    /// <param name="arrow"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public static ExpressionSyntax? GetReturnExpression(ArrowExpressionClauseSyntax? arrow, BlockSyntax? body)
    {
        if (arrow is not null)
            return arrow.Expression;
        if (body is not null && body.Statements.Count == 1 && body.Statements[0] is ReturnStatementSyntax returnStatement)
        {
            // 只有return才使用
            var expression = returnStatement.Expression;
            if (expression is not null)
                return expression;
        }
        return null;
    }
}
