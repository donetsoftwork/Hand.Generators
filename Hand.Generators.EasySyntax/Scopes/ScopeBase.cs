using Hand.Collections;
using Hand.Tidy;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Hand.Scopes;

/// <summary>
/// 作用域
/// </summary>
public abstract class ScopeBase
   : ISyntaxScope, ITypeSimplifier
{
    #region 配置
    /// <inheritdoc />
    public abstract NameSyntax Name { get; }
    /// <summary>
    /// 展示的类名
    /// </summary>
    protected readonly Dictionary<string, NameSyntax> _metadataNames = [];
    #endregion

    /// <inheritdoc />
    public abstract void Using(UsingDirectiveSyntax directive);
    /// <inheritdoc />
    public virtual void Using(NameSyntax @namespace)
    {
        var directive = SyntaxFactory.UsingDirective(@namespace);
        Using(directive);
    }
    /// <inheritdoc />
    public abstract void Add(ITypeGenerator type);
    /// <inheritdoc />
    public abstract CompilationUnitSyntax BuildUnit();
    #region ISyntaxScope
    /// <inheritdoc />
    public virtual TypeSyntax Display(INamedTypeSymbol symbol)
    {
        var containingNamespace = symbol.ContainingNamespace.ToName();
        var type = GetTypeName(symbol);
        var metadataName = symbol.MetadataName;
        if (TryGetNamespace(metadataName, out var @namespace))
        {
            if (!TypeComparer.Equal(containingNamespace, @namespace))
                return type.Qualify(containingNamespace);
        }
        else
        {
            _metadataNames[metadataName] = containingNamespace;
            Using(containingNamespace);
        }
        return type;
    }
    /// <inheritdoc />
    public virtual NameSyntax Display(string typeName, NameSyntax @namespace)
    {
        if(TypeComparer.IsEmpty(@namespace))
            return SyntaxFactory.IdentifierName(typeName);

        if (TryGetNamespace(typeName, out var @namespace0))
        {
            if (TypeComparer.Equal(@namespace0, @namespace))
                return SyntaxFactory.IdentifierName(typeName);
            return SyntaxFactory.IdentifierName(typeName).Qualify(@namespace);
        }
        _metadataNames[typeName] = @namespace;
        Using(@namespace);
        return SyntaxFactory.IdentifierName(typeName);
    }
    /// <summary>
    /// 展示泛型
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public virtual NameSyntax GenericDisplay(string typeName, NameSyntax @namespace, params TypeSyntax[] arguments)
    {
        var metadataName = $"{typeName}`{arguments.Length}";
        if (TryGetNamespace(metadataName, out var @namespace0))
        {
            if (TypeComparer.Equal(@namespace0, @namespace))
                return SyntaxGenerator.Generic(typeName, arguments);
            return SyntaxGenerator.Generic(typeName, arguments).Qualify(@namespace);
        }
        _metadataNames[metadataName] = @namespace;
        Using(@namespace);
        return SyntaxGenerator.Generic(typeName, arguments);
    }
    /// <summary>
    /// 展示泛型
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public virtual NameSyntax GenericDisplay(string typeName, NameSyntax @namespace, TypeArgumentListSyntax arguments)
    {
        var metadataName = $"{typeName}`{arguments.Arguments.Count}";
        if (TryGetNamespace(metadataName, out var @namespace0))
        {
            if (TypeComparer.Equal(@namespace0, @namespace))
                return SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeName), arguments);
            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeName), arguments).Qualify(@namespace);
        }
        _metadataNames[metadataName] = @namespace;
        Using(@namespace);
        return SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeName), arguments);
    }
    /// <inheritdoc />
    public virtual NameSyntax DisplayMember(string typeName, NameSyntax @namespace, SimpleNameSyntax memberName)
        => memberName.Qualify(Display(typeName, @namespace));
    #endregion
    /// <summary>
    /// 获取类型名(支持泛型定义不支持闭合泛型)
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    private static SimpleNameSyntax GetTypeName(INamedTypeSymbol symbol)
    {
        if (symbol.IsGenericType)
        {
            var parameters = symbol.TypeParameters;
            var count = parameters.Length;
            var arguments = new TypeSyntax[count];
            for (var i = 0; i < count; i++)
                arguments[i] = SyntaxFactory.IdentifierName(parameters[i].Name);
            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(symbol.Name), SyntaxFactory.TypeArgumentList([.. arguments]));
        }
        return SyntaxFactory.IdentifierName(symbol.Name);
    }
    /// <summary>
    /// 尝试获取命名空间
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="namespace"></param>
    /// <returns></returns>
    public virtual bool TryGetNamespace(string typeName, [NotNullWhen(true)] out NameSyntax? @namespace)
        => _metadataNames.TryGetValue(typeName, out @namespace);

    #region ITypeSimplifier
    /// <inheritdoc />
    public TypeSyntax Simplify(TypeSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => type,
            SyntaxKind.QualifiedName => SimplifyQualified((QualifiedNameSyntax)type),
            SyntaxKind.AliasQualifiedName => SimplifyAlias((AliasQualifiedNameSyntax)type),
            SyntaxKind.GenericName => this.SimplifyArgumentList((GenericNameSyntax)type),
            SyntaxKind.PredefinedType => type,
            SyntaxKind.ArrayType => this.SimplifyArray((ArrayTypeSyntax)type),
            SyntaxKind.NullableType => this.SimplifyNullable((NullableTypeSyntax)type),
            SyntaxKind.OmittedTypeArgument => type,
            SyntaxKind.TupleType => this.SimplifyTuple((TupleTypeSyntax)type),
            SyntaxKind.PointerType => this.SimplifyPointer((PointerTypeSyntax)type),
            SyntaxKind.FunctionPointerType => this.SimplifyPointer((FunctionPointerTypeSyntax)type),
            SyntaxKind.RefType => this.SimplifyRef((RefTypeSyntax)type),
            SyntaxKind.ScopedType => this.SimplifyScoped((ScopedTypeSyntax)type),
            _ => type,
        };
    }
    /// <inheritdoc />
    public NameSyntax Simplify(NameSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => type,
            SyntaxKind.QualifiedName => SimplifyQualified((QualifiedNameSyntax)type),
            SyntaxKind.AliasQualifiedName => SimplifyAlias((AliasQualifiedNameSyntax)type),
            SyntaxKind.GenericName => this.SimplifyArgumentList((GenericNameSyntax)type),
            _ => type,
        };
    }
    /// <summary>
    /// 简化别名
    /// </summary>
    /// <param name="aliasName"></param>
    /// <returns></returns>
    public NameSyntax SimplifyAlias(AliasQualifiedNameSyntax aliasName)
    {
        var alias = aliasName.Alias;
        var name = SimplifySimple(aliasName.Name);
        if (TypeComparer.IsGlobal(alias))
            return name;
        return SyntaxFactory.AliasQualifiedName(alias, name);
    }
    /// <summary>
    /// 简化限定名
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public NameSyntax SimplifyQualified(QualifiedNameSyntax type)
    {
        var name = type.Right;
        if (name.IsKind(SyntaxKind.GenericName))
        {
            var genericName = this.SimplifyArgumentList((GenericNameSyntax)name);
            return GenericDisplay(genericName.Identifier.Text, type.Left, genericName.TypeArgumentList);
        }
        else
        {
            var identifierName = (IdentifierNameSyntax)name;
            return Display(identifierName.Identifier.Text, type.Left);
        }
    }
    /// <summary>
    /// 简化简单名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SimpleNameSyntax SimplifySimple(SimpleNameSyntax name)
        => name.IsKind(SyntaxKind.IdentifierName) ? name : this.SimplifyArgumentList((GenericNameSyntax)name);
    #endregion
}
