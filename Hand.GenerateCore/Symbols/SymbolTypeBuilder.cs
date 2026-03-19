using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 类型信息构造器
/// </summary>
public class SymbolTypeBuilder
{
    #region 配置
    private readonly Dictionary<string, IFieldSymbol> _fields = [];
    private readonly Dictionary<string, IPropertySymbol> _properties = [];
    private readonly List<IMethodSymbol> _constructors = [];
    private readonly List<IMethodSymbol> _operators = [];
    private readonly List<IMethodSymbol> _methods = [];
    private readonly SymbolCompositeCollect _collect = new();
    #endregion
    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public SymbolTypeDescriptor Build(Compilation compilation, INamedTypeSymbol symbol)
    {
        foreach (var member in symbol.GetMembers())
            _collect.Add(member);
        return new(compilation, symbol, _fields, _properties, _constructors, _operators, _methods);
    }
    /// <summary>
    /// 需要字段
    /// </summary>
    /// <returns></returns>
    public SymbolTypeBuilder WithField()
    {
        if (_collect.ContainsKey(_fields))
            return this;
        var collect = new SymbolDictionaryCollect<IFieldSymbol>(SymbolKind.Field, _fields);
        _collect.Use(_fields, collect);
        return this;
    }
    /// <summary>
    /// 需要属性
    /// </summary>
    /// <returns></returns>
    public SymbolTypeBuilder WithProperty()
    {
        if (_collect.ContainsKey(_properties))
            return this;
        var collect = new SymbolDictionaryCollect<IPropertySymbol>(SymbolKind.Property, _properties);
        _collect.Use(_properties, collect);
        return this;
    }
    /// <summary>
    /// 需要运算符重载(在WithMethod前调用)
    /// </summary>
    public SymbolTypeBuilder WithConstructor()
    {
        if (_collect.ContainsKey(_constructors))
            return this;
        var collect = new ConstructorCollect(_constructors);
        _collect.Use(_constructors, collect);
        return this;
    }
    /// <summary>
    /// 需要运算符重载(在WithMethod前调用)
    /// </summary>
    public SymbolTypeBuilder WithOperator()
    {
        if (_collect.ContainsKey(_operators))
            return this;
        var collect = new OperatorCollect(_operators);
        _collect.Use(_operators, collect);
        return this;
    }
    /// <summary>
    /// 需要方法
    /// </summary>
    public SymbolTypeBuilder WithMethod()
    {
        if (_collect.ContainsKey(_methods))
            return this;
        var collect = new MethodCollect(_methods);
        _collect.Use(_methods, collect);
        return this;
    }
}
