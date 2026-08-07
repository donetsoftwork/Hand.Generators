namespace GenerateConvertTests.Supports;

public static partial class MyColorServices
{
    public static ConsoleColor ToConsoleColor(this MyColor color)
        => color switch
        {
            MyColor.Red => ConsoleColor.Red,
            MyColor.Green => ConsoleColor.Green,
            MyColor.Blue => ConsoleColor.Blue,
            _ => default
        };
    public static string ToString(this MyColor color)
        => color switch
        {
            MyColor.Red => "Red",
            MyColor.Green => "Green",
            MyColor.Blue => "Blue",
            _ => color.ToString()
        };
    //public static MyColor ToMyColor(this string color)
    //    => color switch
    //    {
    //        "Red" => MyColor.Red,
    //        "Green" => MyColor.Green,
    //        "Blue" => MyColor.Blue,
    //        _ => default
    //    };
}
