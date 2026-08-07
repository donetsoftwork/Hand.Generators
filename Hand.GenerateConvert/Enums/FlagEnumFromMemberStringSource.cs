using Hand.Methods;
using Hand.Reflection;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Enums;

/// <summary>
/// 特殊位域枚举成员转string
/// </summary>
/// <param name="compilation">编译信息</param>
/// <param name="enumInfo">枚举信息</param>
/// <param name="methodName">方法名</param>
/// <param name="members">特殊枚举成员</param>
public class FlagEnumFromMemberStringSource(Compilation compilation, TypeSymbolInfo enumInfo, string methodName, IEnumField[] members)
    : EnumFromMemberStringSource(compilation, enumInfo, methodName, members)
{
    private static readonly ExpressionSyntax _separator = SyntaxGenerator.Collection(SyntaxGenerator.Literal(','), SyntaxGenerator.Literal(' '));
    private static readonly ExpressionSyntax _options = SyntaxFactory.IdentifierName(nameof(StringSplitOptions)).Access(nameof(StringSplitOptions.RemoveEmptyEntries));
    /// <inheritdoc />
    public override MethodDeclarationSyntax BuildBody(MethodDeclarationSyntax method, ExpressionSyntax @this)
    {
        var localFunction = SyntaxFactory.IdentifierName("Local" + _methodName);
        var localBuilder = _returnType.LocalFunction(localFunction.Identifier, SyntaxGenerator.StringType.Parameter(ExtensionMethodSource.ExtensionThis.Identifier))
            .Static()
            .ToBuilder();
        var builder = method.ToBuilder()
            .Add(BuildBody(localBuilder, @this))
            .If(StringCompareMethods.IsNullOrWhiteSpace(@this))
            .Add(SyntaxGenerator.DefaultLiteral.Return())
            .End();
        var result = SyntaxFactory.IdentifierName("result");
        // TEnum result = default;
        builder.Declare(_returnType.Variable(result.Identifier, SyntaxGenerator.DefaultLiteral));
        var list = SyntaxFactory.IdentifierName("list");
        // var list = @this.Split([',', ' ']);
        builder.Declare(SyntaxGenerator.VarType.Variable(list.Identifier, @this.Access(nameof(string.Split)).Invocation([_separator, _options])));
        var item = SyntaxFactory.IdentifierName("item");
        // foreach (var item in list)
        return builder.ForEach(item.Identifier, list)
            // result |= LocalFunction(item);
            .AddExpression(result.OrAssign(localFunction.Invocation([item])))
            .End()
        // return result;
        .Return(result);
    }
}
