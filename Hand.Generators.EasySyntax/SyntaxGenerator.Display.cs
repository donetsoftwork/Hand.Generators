//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System.Collections.Generic;

//namespace Hand;

///// <summary>
///// 展示
///// </summary>
//partial class SyntaxGenerator
//{
//    #region 配置
//    /// <summary>
//    /// 类名
//    /// </summary>
//    protected readonly string _typeName = declaration.Identifier.Text;
//    /// <summary>
//    /// 类名
//    /// </summary>
//    private readonly Dictionary<string, string> _metadataNames = [];
//    #endregion

//    ///// <summary>
//    ///// 展示类名
//    ///// 支持泛型定义不支持闭合泛型
//    ///// 闭合泛型请使用Hand.GenerateCore的扩展方法TypeSyntax Display(this SyntaxGenerator generator, GenericTypeInfo info)
//    ///// 暂不支持内部类
//    ///// </summary>
//    ///// <param name="symbol"></param>
//    ///// <param name="isNullable"></param>
//    ///// <returns></returns>
//    //public TypeSyntax Display(INamedTypeSymbol symbol, bool isNullable)
//    //    => Display(symbol).Nullable(isNullable);
//    /// <summary>
//    /// 展示类名
//    /// 支持泛型定义不支持闭合泛型
//    /// 闭合泛型请使用Hand.GenerateCore的扩展方法TypeSyntax Display(this SyntaxGenerator generator, GenericTypeInfo info)
//    /// 暂不支持内部类
//    /// </summary>
//    /// <param name="symbol"></param>
//    /// <returns></returns>
//    public TypeSyntax Display(INamedTypeSymbol symbol)
//    {
//        var containingNamespace = symbol.ContainingNamespace.ToDisplayString();
//        SimpleNameSyntax type = GetTypeName(symbol);
//        var metadataName = symbol.MetadataName;
//        if (TryGetNamespace(metadataName, out var @namespace))
//        {
//            if (!string.Equals(containingNamespace, @namespace))
//                return type.Qualify(containingNamespace);
//        }
//        else
//        {
//            _metadataNames[metadataName] = containingNamespace;
//            Using(containingNamespace);
//        }
//        return type;
//    }
//    /// <summary>
//    /// 获取类型名(支持泛型定义不支持闭合泛型)
//    /// </summary>
//    /// <param name="symbol"></param>
//    /// <returns></returns>
//    public static SimpleNameSyntax GetTypeName(INamedTypeSymbol symbol)
//    {
//        if (symbol.IsGenericType)
//        {
//            var parameters = symbol.TypeParameters;
//            var count = parameters.Length;
//            var arguments = new TypeSyntax[count];
//            for (var i = 0; i < count; i++)
//                arguments[i] = SyntaxFactory.IdentifierName(parameters[i].Name);
//            return SyntaxFactory.GenericName(SyntaxFactory.Identifier(symbol.Name), SyntaxFactory.TypeArgumentList([.. arguments]));
//        }
//        return SyntaxFactory.IdentifierName(symbol.Name);
//    }
//    /// <summary>
//    /// 展示类名
//    /// </summary>
//    /// <param name="typeName"></param>
//    /// <param name="namespace"></param>
//    /// <returns></returns>
//    public NameSyntax Display(string typeName, string @namespace)
//    {
//        if (TryGetNamespace(typeName, out var @namespace0))
//        {
//            if (string.Equals(@namespace0, @namespace))
//                return SyntaxFactory.IdentifierName(typeName);
//            return SyntaxFactory.IdentifierName(typeName).Qualify(@namespace);
//        }
//        _metadataNames[typeName] = @namespace;
//        Using(@namespace);
//        return SyntaxFactory.IdentifierName(typeName);
//    }
//    /// <summary>
//    /// 展示类成员
//    /// </summary>
//    /// <param name="typeName"></param>
//    /// <param name="namespace"></param>
//    /// <param name="memberName"></param>
//    /// <returns></returns>
//    public NameSyntax DisplayMember(string typeName, string @namespace, SimpleNameSyntax memberName)
//    {
//        if (TryGetNamespace(typeName, out var @namespace0))
//        {
//            if (string.Equals(@namespace0, @namespace))
//            {
//                if (string.Equals(typeName, _typeName))
//                    return memberName;
//                return memberName.Qualify(SyntaxFactory.IdentifierName(typeName));
//            }
//            return memberName.Qualify(SyntaxFactory.IdentifierName(typeName).Qualify(@namespace));
//        }
//        _metadataNames[typeName] = @namespace;
//        Using(@namespace);
//        return memberName.Qualify(SyntaxFactory.IdentifierName(typeName));
//    }
//    ///// <summary>
//    ///// 展示泛型方法
//    ///// </summary>
//    ///// <param name="typeName"></param>
//    ///// <param name="namespace"></param>
//    ///// <param name="methodName"></param>
//    ///// <param name="arguments"></param>
//    ///// <returns></returns>
//    //public NameSyntax GenericMethod(string typeName, string @namespace, SyntaxToken methodName, params TypeSyntax[] arguments)
//    //    => DisplayMember(typeName, @namespace, Generic(methodName, arguments));
//    /// <summary>
//    /// 展示泛型
//    /// </summary>
//    /// <param name="typeName"></param>
//    /// <param name="namespace"></param>
//    /// <param name="arguments"></param>
//    /// <returns></returns>
//    public NameSyntax GenericDisplay(string typeName, string @namespace, params TypeSyntax[] arguments)
//    {
//        var metadataName = $"{typeName}`{arguments.Length}";
//        if (TryGetNamespace(metadataName, out var @namespace0))
//        {
//            if (string.Equals(@namespace0, @namespace))
//                return Generic(typeName, arguments);
//            return Generic(typeName, arguments).Qualify(@namespace);
//        }
//        _metadataNames[metadataName] = @namespace;
//        Using(@namespace);
//        return Generic(typeName, arguments);
//    }
//    /// <summary>
//    /// 尝试获取命名空间
//    /// </summary>
//    /// <param name="typeName"></param>
//    /// <param name="namespace"></param>
//    /// <returns></returns>
//    protected virtual bool TryGetNamespace(string typeName, out string? @namespace)
//        => _metadataNames.TryGetValue(typeName, out @namespace);
//}
