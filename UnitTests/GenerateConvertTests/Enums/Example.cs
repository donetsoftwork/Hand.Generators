// EnumFromMember
//namespace System
//{
//    internal static partial class StringExtensions
//    {
//        internal static global::GenerateConvertTests.DTO.MyColorDTO ToMyColorDTO(this string @this)
//        {
//            if (string.Equals(@this, "R", System.StringComparison.OrdinalIgnoreCase))
//                return global::GenerateConvertTests.DTO.MyColorDTO.Red;
//            if (string.Equals(@this, "G", System.StringComparison.OrdinalIgnoreCase))
//                return global::GenerateConvertTests.DTO.MyColorDTO.Green;
//            if (string.Equals(@this, "B", System.StringComparison.OrdinalIgnoreCase))
//                return global::GenerateConvertTests.DTO.MyColorDTO.Blue;
//            return System.Enum.Parse<global::GenerateConvertTests.DTO.MyColorDTO>(@this, true);
//        }
//    }
//}
// FlagEnumFromMember
//namespace System
//{
//    internal static partial class StringExtensions
//    {
//        internal static global::GenerateConvertTests.Supports.MyColor ToMyColor(this string @this)
//        {
//            static global::GenerateConvertTests.Supports.MyColor LocalToMyColor(string @this)
//            {
//                if (string.Equals(@this, "R", System.StringComparison.OrdinalIgnoreCase))
//                    return global::GenerateConvertTests.Supports.MyColor.Red;
//                if (string.Equals(@this, "G", System.StringComparison.OrdinalIgnoreCase))
//                    return global::GenerateConvertTests.Supports.MyColor.Green;
//                if (string.Equals(@this, "B", System.StringComparison.OrdinalIgnoreCase))
//                    return global::GenerateConvertTests.Supports.MyColor.Blue;
//                return System.Enum.Parse<global::GenerateConvertTests.Supports.MyColor>(@this, true);
//            }

//            if (string.IsNullOrWhiteSpace(@this))
//                return default;
//            global::GenerateConvertTests.Supports.MyColor result = default;
//            var list = @this.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
//            foreach (var item in list)
//                result |= LocalToMyColor(item);
//            return result;
//        }
//    }
//}
// EnumToEnum
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
// FlagToFlag
//namespace GenerateConvertTests.Supports
//{
//    internal static partial class ColumnTypeExtensions
//    {
//        internal static global::GenerateConvertTests.DTO.ColumnTypeDTO ToDTO(this ColumnType @this)
//        {
//            ulong result = 0UL;
//            if ((@this & ColumnType.Identity) == ColumnType.Identity)
//                result |= 1UL;
//            if ((@this & ColumnType.Key) == ColumnType.Key)
//                result |= 2UL;
//            if ((@this & ColumnType.Unique) == ColumnType.Unique)
//                result |= 4UL;
//            if ((@this & ColumnType.NotNull) == ColumnType.NotNull)
//                result |= 8UL;
//            if ((@this & ColumnType.Computed) == ColumnType.Computed)
//                result |= 16UL;
//            return (global::GenerateConvertTests.DTO.ColumnTypeDTO)result;
//        }
//    }
//}
// FlagToEnum
//namespace GenerateConvertTests.Supports
//{
//    internal static partial class MyColorExtensions
//    {
//        internal static global::System.ConsoleColor ToConsoleColor(this MyColor @this)
//        {
//            if ((@this & MyColor.Red) == MyColor.Red)
//                return global::System.ConsoleColor.Red;
//            if ((@this & MyColor.Green) == MyColor.Green)
//                return global::System.ConsoleColor.Green;
//            if ((@this & MyColor.Blue) == MyColor.Blue)
//                return global::System.ConsoleColor.Blue;
//            return default;
//        }
//    }
//}
// ComplexToComplex
//namespace GeneratePocoTests.Supports
//{
//    internal static partial class UserExtensions
//    {
//        internal static global::GenerateConvertTests.DTO.UserDTO ToDTO(this User @this) => new()
//        {
//            Id = @this.Id,
//            Name = @this.Name,
//            Sex = @this.Sex
//        };
//    }
//}
//namespace GenerateConvertTests.DTO
//{
//    internal static partial class UserDTOExtensions
//    {
//        internal static global::GeneratePocoTests.Supports.User ToUser(this UserDTO @this) => new(@this.Id, @this.Name, @this.Sex);
//    }
//}
//namespace GeneratePocoTests.Supports
//{
//    internal static partial class UserExtensions
//    {
//        internal static global::GenerateConvertTests.Supports.UserEntity ToEntity(this User @this) => new(new global::GenerateConvertTests.Supports.UserId(@this.Id), new global::GenerateConvertTests.Supports.UserName(@this.Name));
//    }
//}