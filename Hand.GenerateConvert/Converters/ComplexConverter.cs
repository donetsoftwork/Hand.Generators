//using Hand.Arguments;
//using Hand.Builders;
//using Hand.Members;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace Hand.Converters;

///// <summary>
///// 复杂转化器
///// </summary>
///// <param name="builder"></param>
///// <param name="complexType"></param>
///// <param name="arguments"></param>
//public class ComplexConverter(ConvertBuilder builder, TypeSyntax complexType, MemberArgument[] arguments)
//    : IConverter
//{
//    #region 配置
//    private readonly ConvertBuilder _builder = builder;
//    private readonly TypeSyntax _complex = complexType;
//    private readonly MemberArgument[] _arguments = arguments;
//    #endregion

//    /// <inheritdoc />
//    public ExpressionSyntax Convert(ExpressionSyntax source)
//    {
//        var builder = new CreationBuilder(_complex);
//        foreach (var argument in _arguments)
//        {
//            var parameter = argument.Member;
//            var sourceMember = argument.Source;
//            var memberConverter = sourceMember is null ? null : _builder.Get(sourceMember.MemberSymbol.Symbol, parameter.MemberSymbol.Symbol);
//            if (parameter.Kind == SymbolKind.Parameter)
//            {
//                if (sourceMember is null)
//                {
//                    if (parameter.IsOptional)
//                        continue;
//                    builder.WithArgument(SyntaxGenerator.DefaultLiteral);
//                }
//                else if(memberConverter is null)
//                {
//                    builder.WithArgument(SyntaxGenerator.DefaultLiteral);
//                }
//                else if (parameter.IsOptional)
//                {
//                    builder.WithArgument(parameter.Name, memberConverter.Convert(source.Access(sourceMember.Name)));
//                }
//                else
//                {
//                    builder.WithArgument(memberConverter.Convert(source.Access(sourceMember.Name)));
//                }
//            }
//            else if (sourceMember is not null && memberConverter is not null)
//            {
//                builder.Initialize(parameter.Name, memberConverter.Convert(source.Access(sourceMember.Name)));
//            }
//        }
//        return builder.Build();
//    }
//    //public ExpressionSyntax CheckType(INamedTypeSymbol source, INamedTypeSymbol dest, ExpressionSyntax original)
//    //{
//    //    if(source.Equals(dest, SymbolEqualityComparer.Default))
//    //        return original;
//    //    var converter = _builder.Get(source, dest);
//    //    return converter.Convert(original);
//    //}
//}
