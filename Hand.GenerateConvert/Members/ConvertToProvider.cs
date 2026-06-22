//using Hand.Symbols;
//using Microsoft.CodeAnalysis;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Hand.Members;

///// <summary>
///// 
///// </summary>
//public class ConvertToProvider
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="compilation"></param>
//    /// <param name="source"></param>
//    /// <param name="dest"></param>
//    /// <returns></returns>
//    public static IMethodSymbol? GetConvertMethod(Compilation compilation, INamedTypeSymbol source, INamedTypeSymbol dest)
//    {
//        // ISymbolMethod?
//        var sourceName = source.Name;
//        var destName = dest.Name;

//        var convertToInfo = ConvertMethodInfo.Create(destName, sourceName, "To");
//        var sourceMethods = SymbolReflection.GetMethods(source)
//            .Where(m => !m.IsStatic && SymbolTypeDescriptor.CheckEquals(dest, m.ReturnType));
//        // 优先source的ConvertTo方法
//        var method = GetMethod(sourceMethods, convertToInfo.Filter);
//        if (method is not null)
//            return method;

//        var convertFromInfo = ConvertMethodInfo.Create(sourceName, destName, "From");
//        var destMethods = SymbolReflection.GetMethods(dest)
//            .Where(m => m.IsStatic && SymbolTypeDescriptor.CheckEquals(dest, m.ReturnType) && SymbolTypeDescriptor.MatchFirst(m.Parameters, source));
//        // dest的ConvertFrom方法
//        method = GetMethod(destMethods, convertFromInfo.Filter);
//        if (method is not null)
//            return method;
//        var sourceExtensionInfo = TypeNameInfo.GetExtensionInfo(source);
//        var sourceExtension = compilation.GetTypeByMetadataName(sourceExtensionInfo.FullName);
//        if (sourceExtension is not null)
//        {
//            sourceMethods = SymbolReflection.GetMethods(source)
//                .Where(m => m.IsStatic && SymbolTypeDescriptor.CheckEquals(dest, m.ReturnType) && SymbolTypeDescriptor.MatchFirst(m.Parameters, source));
//            // source扩展类的ConvertTo方法
//            method = GetMethod(sourceMethods, convertToInfo.Filter);
//            if (method is not null)
//                return method;
//        }
//        return null;
//    }
//    /// <summary>
//    /// 获取参数最好的方法
//    /// </summary>
//    /// <param name="methods"></param>
//    /// <param name="filter"></param>
//    /// <returns></returns>
//    public static IMethodSymbol? GetMethod(IEnumerable<IMethodSymbol> methods, Func<IMethodSymbol, bool> filter)
//        => methods.OrderBy(m => m.Parameters.Length)
//        .FirstOrDefault(filter);
//    ///// <summary>
//    ///// 按ConvertTo查找
//    ///// </summary>
//    ///// <param name="source"></param>
//    ///// <param name="dest"></param>
//    ///// <returns></returns>
//    //public static Func<IMethodSymbol, bool> FilterByConvertTo(string source, string dest)
//    //{
//    //    var name = "To" + dest;        
//    //    if(dest.StartsWith(source, StringComparison.OrdinalIgnoreCase‌))
//    //    {
//    //        var name2 = "To" + dest.Substring(source.Length);
//    //        return m => EqualName(m, name) || EqualName(m, name2);
//    //    }
//    //    return m => EqualName(m, name);
//    //}
//    ///// <summary>
//    ///// 按ConvertFrom查找
//    ///// </summary>
//    ///// <param name="source"></param>
//    ///// <param name="dest"></param>
//    ///// <returns></returns>
//    //public static Func<IMethodSymbol, bool> FilterByConvertFrom(string source, string dest)
//    //{
//    //    var name = "From" + source;
//    //    if (source.StartsWith(dest, StringComparison.InvariantCultureIgnoreCase))
//    //    {
//    //        var name2 = "From" + source.Substring(dest.Length);
//    //        return m => EqualName(m, name) || EqualName(m, name2);
//    //    }
//    //    return m => EqualName(m, name);
//    //}
//    ///// <summary>
//    ///// 忽略大小写比较Name
//    ///// </summary>
//    ///// <param name="symbol"></param>
//    ///// <param name="name"></param>
//    ///// <returns></returns>
//    //public static bool EqualName(ISymbol symbol, string name)
//    //    => string.Equals(symbol.Name, name, StringComparison.InvariantCultureIgnoreCase);
//    //public static IMethodSymbol? GetConvertToMethod(INamedTypeSymbol source, INamedTypeSymbol dest)
//    //{

//    //}
//}
