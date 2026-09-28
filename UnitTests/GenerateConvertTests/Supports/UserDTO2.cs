using GenerateConvertTests.DTO;

namespace GenerateConvertTests.Supports;

public partial class UserDTO2 : UserDTO
{
    public User ToUser2()
        => new(Id, Name, Sex);
}
