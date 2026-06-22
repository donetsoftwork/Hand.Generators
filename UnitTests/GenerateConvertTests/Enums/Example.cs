//namespace System
//{
//    internal static partial class StringExtensions
//    {
//        internal static global::GenerateConvertTests.Supports.MyColor ToMyColor(this string @this)
//        {
//            if (string.Equals(@this, "R", System.StringComparison.OrdinalIgnoreCase))
//                return global::GenerateConvertTests.Supports.MyColor.Red;
//            if (string.Equals(@this, "G", System.StringComparison.OrdinalIgnoreCase))
//                return global::GenerateConvertTests.Supports.MyColor.Green;
//            if (string.Equals(@this, "B", System.StringComparison.OrdinalIgnoreCase))
//                return global::GenerateConvertTests.Supports.MyColor.Blue;
//            return System.Enum.Parse<global::GenerateConvertTests.Supports.MyColor>(@this, true);
//        }
//    }
//}

//namespace System
//{
//    internal static partial class ConsoleColorExtensions
//    {
//        internal static global::GenerateConvertTests.Supports.MyColor ToMyColor(this ConsoleColor @this)
//        {
//            return @this switch
//            {
//                ConsoleColor.Blue => global::GenerateConvertTests.Supports.MyColor.Blue,
//                ConsoleColor.Green => global::GenerateConvertTests.Supports.MyColor.Green,
//                ConsoleColor.Red => global::GenerateConvertTests.Supports.MyColor.Red,
//                _ => default
//            };
//        }
//    }
//}