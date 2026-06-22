//using Hand.Builders;
//using Hand.Converters;
//using Hand.Members;
//using Hand.Symbols;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Hand.Enums;

//public class EnumFromStringProvider
//{
//    public IConverter GetConverter(Compilation compilation, INamedTypeSymbol enumType, IEnumBundle bundle)
//    {
//        var fileds = bundle.Fields
//            .Where(field => !string.IsNullOrWhiteSpace(field.Member))
//            .ToArray();
//        if (fileds.Length > 0)
//        {
//            var stringType = compilation.GetStringSymbol();
//            var extensionInfo = TypeNameInfo.GetExtensionInfo(enumType);
//            var extension = compilation.GetTypeByMetadataName(extensionInfo.FullName);
//            var methodInfo = ConvertMethodInfo.Create(enumType.Name, stringType.Name, "To");
//            var methodName = methodInfo.Name;
//            if (extension is null)
//            {
//                var source = new EnumFromMemberStringSource(compilation, enumType.ToSyntax(), extensionInfo, methodName, fileds);
//                //_convertBuilder.AddSource(source);
//                return new StaticMethodConverter(SyntaxFactory.IdentifierName(extensionInfo.FullName).Access(methodName));
//            }
//            else if(ConvertBuilder.GetStaticMethod(extension, stringType, enumType, methodInfo.Filter) is IMethodSymbol method)
//            {
//                if(method.Parameters.Length == 1)
//                    return new StaticMethodConverter(SyntaxFactory.IdentifierName(extensionInfo.FullName).Access(methodName));
//                return new EnumParseConverter(enumType.ToSyntax(), true);
//            }
//            //else if(extension.IsPartial)
//            //{

//            //}
//        }
//        // 默认通过Enum.Parse转化
//        return new EnumParseConverter(enumType.ToSyntax(), true);
//    }
//}
