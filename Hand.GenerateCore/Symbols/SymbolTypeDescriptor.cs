using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Hand.Symbols;

/// <summary>
/// 类型信息
/// </summary>
public class SymbolTypeDescriptor(Compilation compilation, INamedTypeSymbol symbol, Dictionary<string, IFieldSymbol> fields, Dictionary<string, IPropertySymbol> properties, List<IMethodSymbol> constructors, List<IMethodSymbol> operators, List<IMethodSymbol> methods)
{
    #region 配置
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol _symbol = symbol;
    private readonly Dictionary<string, IFieldSymbol> _fields = fields;
    private readonly Dictionary<string, IPropertySymbol> _properties = properties;
    private readonly List<IMethodSymbol> _constructors = constructors;
    private readonly List<IMethodSymbol> _operators = operators;
    private readonly List<IMethodSymbol> _methods = methods;
    /// <summary>
    /// 编译信息
    /// </summary>
    public Compilation Compilation 
        => _compilation;
    /// <summary>
    /// 类型
    /// </summary>
    public INamedTypeSymbol Symbol 
        => _symbol;
    /// <summary>
    /// 构造函数
    /// </summary>
    public IEnumerable<IMethodSymbol> Constructors 
        => _constructors;
    /// <summary>
    /// 字段
    /// </summary>
    public IDictionary<string, IFieldSymbol> Fields
        => _fields;
    /// <summary>
    /// 属性
    /// </summary>
    public IDictionary<string, IPropertySymbol> Properties
        => _properties;
    /// <summary>
    /// 方法
    /// </summary>
    public IEnumerable<IMethodSymbol> Methods
        => _methods;
    #endregion
    /// <summary>
    /// 获取INamedTypeSymbol
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public INamedTypeSymbol? GetSymbol(string typeFullName)
        => _compilation.GetTypeByMetadataName(typeFullName);
    /// <summary>
    /// 把Type转INamedTypeSymbol
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public INamedTypeSymbol? GetSymbol(Type type)
        => _compilation.GetTypeByMetadataName(type.FullName);
    //public INamedTypeSymbol? GetSymbol(SyntaxNode declaration)
    //    => _compilation.GetDeclaredSymbolForNode
    ///// <summary>
    ///// 把Type转INamedTypeSymbol
    ///// </summary>
    ///// <param name="types"></param>
    ///// <returns></returns>
    //public INamedTypeSymbol[] GetSymbols(Type[] types)
    //{
    //    var count = types.Length;
    //    if (count == 0)
    //        return [];
    //    var symbols = new INamedTypeSymbol[count];
    //    for (int i = 0; i < count; i++)
    //    {
    //        var type = types[i];
    //        var symbol = GetSymbol(type) ?? throw new NotSupportedException(type.FullName);
    //        symbols[i] = symbol;
    //    }
    //    return symbols;
    //}
    /// <summary>
    /// 获取标记值
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue? GetAttributeValue<TAttribute, TValue>(ISymbol symbol, string key)
        where TAttribute : Attribute
        => GetAttributeValue<TAttribute, TValue>(symbol, key);
    /// <summary>
    /// 获取构造函数
    /// </summary>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public IMethodSymbol? GetConstructor(params INamedTypeSymbol[] parameterTypes)
    {
        foreach (var constructor in _constructors)
        {
            if(MatchParameterType(constructor.Parameters, parameterTypes))
                return constructor;
        }
        return null;
    }
    /// <summary>
    /// 获取字段
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IFieldSymbol? GetField(string name)
    {
        _fields.TryGetValue(name, out var field);
        return field;
    }
    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IPropertySymbol? GetProperty(string name)
    {
        _properties.TryGetValue(name, out var property);
        return property;

    }
    #region Method
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isPartial"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public IMethodSymbol? GetMethod(string name, bool isPartial, params INamedTypeSymbol[] parameterTypes)
    {
        foreach (var method in _methods)
        {
            if(method.Name == name && method.IsPartialDefinition == isPartial && MatchParameterType(method.Parameters, parameterTypes))
                return method;
        }
        return null;
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public IMethodSymbol? GetMethod(string name, params INamedTypeSymbol[] parameterTypes)
    {
        foreach (var item in _methods)
        {
            if (item.Name == name && MatchParameterType(item.Parameters, parameterTypes))
                return item;
        }
        return null;
    }
    /// <summary>
    /// 按返回类型获取方法
    /// </summary>
    /// <param name="returnType"></param>
    /// <returns></returns>
    public IEnumerable<IMethodSymbol> GetMethodsByReturnType(INamedTypeSymbol returnType)
        => _methods.Where(item => CheckEquals(returnType, item.ReturnType));
    #endregion
    #region Operator
    /// <summary>
    /// 获取运算符重载
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public IMethodSymbol? GetOperator(string name, params INamedTypeSymbol[] parameterTypes)
    {
        foreach (var item in _operators)
        {
            if (item.Name == name && MatchParameterType(item.Parameters, parameterTypes))
                return item;
        }
        return null;
    }
    /// <summary>
    /// 获取相等重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetEqualOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Equality", [_symbol, otherType]);
    /// <summary>
    /// 获取不等重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetUnEqualOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Inequality", [_symbol, otherType]);
    /// <summary>
    /// 获取相加重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetAddOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Addition", [_symbol, otherType]);
    /// <summary>
    /// 获取相减重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetSubtractOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Subtraction", [_symbol, otherType]);
    /// <summary>
    /// 获取相乘重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetMultiplyOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Multiply", [_symbol, otherType]);
    /// <summary>
    /// 获取相除重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetDivideOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Division", [_symbol, otherType]);
    /// <summary>
    /// 获取求余重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetModOperator(INamedTypeSymbol otherType)
        => GetOperator("op_Modulus", [_symbol, otherType]);
    /// <summary>
    /// 获取逻辑与重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetAndOperator(INamedTypeSymbol otherType)
        => GetOperator("op_LogicalAnd", [_symbol, otherType]);
    /// <summary>
    /// 获取逻辑或重载符
    /// </summary>
    /// <param name="otherType"></param>
    /// <returns></returns>
    public IMethodSymbol? GetOrOperator(INamedTypeSymbol otherType)
        => GetOperator("op_LogicalOr", [_symbol, otherType]);
    #endregion
    /// <summary>
    /// 按参数类型匹配
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public static bool MatchParameterType(ImmutableArray<IParameterSymbol> parameters, INamedTypeSymbol[] parameterTypes)
    {
        var typeCount = parameterTypes.Length;
        if (parameters.Length != typeCount)
            return false;
        for (var i = 0; i < typeCount; i++)
        {
            if (!CheckEquals(parameterTypes[i], parameters[i].Type))
                return false;
        }
        return true;
    }
    /// <summary>
    /// 判断相等
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool CheckEquals(INamedTypeSymbol symbol, ITypeSymbol other)
    {
        if (other.Kind == SymbolKind.ErrorType && other is IErrorTypeSymbol error)
            return string.Equals(symbol.MetadataName, error.MetadataName);
        return symbol.Equals(other, SymbolEqualityComparer.Default);
    }
    #region SpecialType
    /// <summary>
    /// bool
    /// </summary>
    public INamedTypeSymbol Bool => _compilation.GetSpecialType(SpecialType.System_Boolean);
    /// <summary>
    /// byte
    /// </summary>
    public INamedTypeSymbol Byte => _compilation.GetSpecialType(SpecialType.System_Byte);
    /// <summary>
    /// sbyte
    /// </summary>
    public INamedTypeSymbol SByte => _compilation.GetSpecialType(SpecialType.System_SByte);
    /// <summary>
    /// int
    /// </summary>
    public INamedTypeSymbol Int => _compilation.GetSpecialType(SpecialType.System_Int32);
    /// <summary>
    /// uint
    /// </summary>
    public INamedTypeSymbol UInt => _compilation.GetSpecialType(SpecialType.System_UInt32);
    /// <summary>
    /// short
    /// </summary>
    public INamedTypeSymbol Short => _compilation.GetSpecialType(SpecialType.System_Int16);
    /// <summary>
    /// ushort
    /// </summary>
    public INamedTypeSymbol UShort => _compilation.GetSpecialType(SpecialType.System_UInt16);
    /// <summary>
    /// long
    /// </summary>
    public INamedTypeSymbol Long => _compilation.GetSpecialType(SpecialType.System_Int64);
    /// <summary>
    /// ulong
    /// </summary>
    public INamedTypeSymbol ULong => _compilation.GetSpecialType(SpecialType.System_UInt64);
    /// <summary>
    /// float
    /// </summary>
    public INamedTypeSymbol Float => _compilation.GetSpecialType(SpecialType.System_Single);
    /// <summary>
    /// double
    /// </summary>
    public INamedTypeSymbol Double => _compilation.GetSpecialType(SpecialType.System_Double);
    /// <summary>
    /// decimal
    /// </summary>
    public INamedTypeSymbol Decimal => _compilation.GetSpecialType(SpecialType.System_Decimal);
    /// <summary>
    /// string
    /// </summary>
    public INamedTypeSymbol String => _compilation.GetSpecialType(SpecialType.System_String);
    /// <summary>
    /// char
    /// </summary>
    public INamedTypeSymbol Char => _compilation.GetSpecialType(SpecialType.System_Char);
    /// <summary>
    /// DateTime
    /// </summary>
    public INamedTypeSymbol DateTime => _compilation.GetSpecialType(SpecialType.System_DateTime);
    /// <summary>
    /// object
    /// </summary>
    public INamedTypeSymbol Object => _compilation.GetSpecialType(SpecialType.System_Object);
    /// <summary>
    /// void
    /// </summary>
    public INamedTypeSymbol Void => _compilation.GetSpecialType(SpecialType.System_Void);
    #endregion
}
