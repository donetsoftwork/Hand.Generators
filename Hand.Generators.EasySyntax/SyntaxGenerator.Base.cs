using Hand.Collections;
using Hand.Scopes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand;

/// <summary>
/// 类型基类
/// </summary>
public abstract class TypeGeneratorBase
    : ScopeBase, ITypeGenerator
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="container"></param>
    /// <param name="typeName"></param>
    public TypeGeneratorBase(ISyntaxScope container, string typeName)
    {
        _container = container;
        _typeName = typeName;
        _name = CheckName(container.Name, SyntaxFactory.IdentifierName(typeName));
        _container.Add(this);
    }
    #region 配置
    private readonly ISyntaxScope _container;
    private readonly string _typeName;
    private readonly NameSyntax _name;
    /// <summary>
    /// 内部类
    /// </summary>
    protected readonly List<ITypeGenerator> _inners = [];
    #endregion
    #region ISyntaxScope
    /// <inheritdoc />
    public override NameSyntax Name 
        => _name;
    /// <summary>
    /// 所属容器
    /// </summary>
    public ISyntaxScope Container 
        => _container;

    /// <inheritdoc />
    public override void Add(ITypeGenerator type)
        => _inners.Add(type);
    /// <inheritdoc />
    public override void Using(UsingDirectiveSyntax directive)
        => _container.Using(directive);
    /// <inheritdoc />
    public override CompilationUnitSyntax BuildUnit()
        => _container.BuildUnit();
    /// <inheritdoc />
    public override void Using(NameSyntax @namespace)
        => _container.Using(@namespace);
    /// <inheritdoc />
    public override NameSyntax DisplayMember(string typeName, NameSyntax @namespace, SimpleNameSyntax memberName)
    {
        if (TryGetNamespace(typeName, out var @namespace0))
        {
            if (string.Equals(@namespace0, @namespace))
            {
                if (string.Equals(typeName, _typeName))
                    return memberName;
                return memberName.Qualify(SyntaxFactory.IdentifierName(typeName));
            }
            return memberName.Qualify(SyntaxFactory.IdentifierName(typeName).Qualify(@namespace));
        }
        _metadataNames[typeName] = @namespace;
        Using(@namespace);
        return memberName.Qualify(SyntaxFactory.IdentifierName(typeName));
    }
    #endregion
    #region ITypeGenerator
    /// <inheritdoc />
    public abstract void AddConstructor(ConstructorDeclarationSyntax constructor);
    /// <inheritdoc />
    public abstract void AddField(FieldDeclarationSyntax field);
    /// <inheritdoc />
    public abstract void AddProperty(PropertyDeclarationSyntax property);
    /// <inheritdoc />
    public abstract void AddMethod(MethodDeclarationSyntax method);
    /// <inheritdoc />
    public abstract void AddOthers(params MemberDeclarationSyntax[] members);
    /// <inheritdoc />
    public abstract void Apply(Func<TypeDeclarationSyntax, TypeDeclarationSyntax> modify);
    /// <inheritdoc />
    public abstract void AddBaseType(BaseTypeSyntax baseType);
    /// <inheritdoc />
    public abstract void AddListParameter(ParameterSyntax parameter);
    /// <inheritdoc />
    public abstract TypeDeclarationSyntax BuildType();
    #endregion
    private static NameSyntax CheckName(NameSyntax container, IdentifierNameSyntax name)
        => TypeComparer.IsEmpty(container) ? name : SyntaxFactory.QualifiedName(container, name);
}
