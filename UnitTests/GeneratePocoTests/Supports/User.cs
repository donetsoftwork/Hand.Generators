using System.ComponentModel.DataAnnotations;

namespace GeneratePocoTests.Supports;

/// <summary>
/// 用户
/// </summary>
/// <param name="Id">Id标识</param>
/// <param name="Name">用户名</param>
/// <param name="Sex">性别</param>
public record User(int Id, [StringLength(100)] string Name, int Sex)
{
    public void PrintInfo()
    {
        Console.WriteLine($"Id: {Id}, Name: {Name}, Sex: {Sex}");
    }
}