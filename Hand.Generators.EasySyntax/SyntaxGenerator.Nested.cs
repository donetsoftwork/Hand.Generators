//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using System.Collections.Generic;

//namespace Hand;

///// <summary>
///// 嵌套内部类构造器
///// </summary>
///// <param name="container"></param>
///// <param name="declaration"></param>
///// <param name="constructors"></param>
///// <param name="fields"></param>
///// <param name="properties"></param>
///// <param name="methods"></param>
//public class NestedGenerator(ISyntaxGenerator container, TypeDeclarationSyntax declaration, List<ConstructorDeclarationSyntax> constructors, List<FieldDeclarationSyntax> fields, List<PropertyDeclarationSyntax> properties, List<MethodDeclarationSyntax> methods)
//    : SyntaxGeneratorBase(declaration, constructors, fields, properties, methods)
//{
//    private readonly ISyntaxGenerator _container = container;

//    /// <summary>
//    /// 添加Using
//    /// </summary>
//    /// <param name="directive"></param>
//    public override void Using(UsingDirectiveSyntax directive)
//        => _container.Using(directive);
//    /// <summary>
//    /// 添加Using
//    /// </summary>
//    /// <param name="namespace"></param>
//    public override void Using(string @namespace)
//        => _container.Using(@namespace);
//}
