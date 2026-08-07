using System.Collections.Generic;

namespace Hand.Members;

/// <summary>
/// 注释
/// </summary>
public class Comment
{
    private string _summary = string.Empty;
    private Dictionary<string, string> _typeParams = [];
    private Dictionary<string, string> _params = [];
    private string _returns = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string Summary 
    {
        get => _summary;
        set => _summary = value;
    }
    /// <summary>
    /// 类型参数
    /// </summary>
    public Dictionary<string, string> TypeParams
    {
        get => _typeParams;
        set => _typeParams = value;
    }
    /// <summary>
    /// 参数
    /// </summary>
    public Dictionary<string, string> Params
    {
        get => _params;
        set => _params = value;
    }
    /// <summary>
    /// 返回值
    /// </summary>
    public string Returns
    {
        get => _returns;
        set => _returns = value;
    }
    /// <summary>
    /// 添加类型参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddTypeParam(string name, string value)
        => _typeParams.Add(name, value);
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddParam(string name, string value)
        => _params.Add(name, value);
}
