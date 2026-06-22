namespace GeneratePocoTests.Supports;

public record User(int Id, string Name, int Sex)
{
    public void PrintInfo()
    {
        Console.WriteLine($"Id: {Id}, Name: {Name}, Sex: {Sex}");
    }
}