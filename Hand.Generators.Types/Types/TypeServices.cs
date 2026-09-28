namespace Hand.Types;

/// <summary>
/// 类型扩展方法
/// </summary>
public static class TypeServices
{
    /// <summary>
    /// 类型简化
    /// </summary>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    public static ITypeSymbolInfo CheckPoco(this ITypeSymbolInfo typeInfo)
    {
        if (typeInfo.Kind == TypeSymbolKind.Entity && typeInfo is EntityPropertyTypeInfo entity)
            return entity.ElementInfo;
        if (typeInfo.Kind == TypeSymbolKind.Enumeration && typeInfo is EnumerationTypeInfo enumeration)
            return enumeration.NameInfo;
        return typeInfo;
    }
}
