namespace Hand.Members;

/// <summary>
/// 生成源信息
/// </summary>
/// <param name="typeInfo"></param>
/// <param name="methodInfo"></param>
/// <param name="isPartial"></param>
/// <param name="isStatic"></param>
public class ConvertSourceInfo(TypeNameInfo typeInfo, ConvertMethodInfo methodInfo, bool isPartial, bool isStatic)
{
    #region 配置
    private readonly TypeNameInfo _typeInfo = typeInfo;
    private readonly ConvertMethodInfo _methodInfo = methodInfo;
    private readonly bool _isPartial = isPartial;
    private readonly bool _isStatic = isStatic;

    /// <summary>
    /// 类型信息
    /// </summary>
    public TypeNameInfo TypeInfo 
        => _typeInfo;
    /// <summary>
    /// 函数信息
    /// </summary>
    public ConvertMethodInfo MethodInfo 
        => _methodInfo;
    /// <summary>
    /// 是否静态方法
    /// </summary>
    public bool IsStatic 
        => _isStatic;
    /// <summary>
    /// 是否部分类
    /// </summary>
    public bool IsPartial 
        => _isPartial;
    #endregion
}
