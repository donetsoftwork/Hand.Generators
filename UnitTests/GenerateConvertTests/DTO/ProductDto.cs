using GenerateConvertTests.Supports;
using Hand.Mapping;

namespace GenerateConvertTests.DTO;


[GenerateConvert<Product>]
public partial class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public MyColorDTO ProductColor { get; set; }
    public UserDTO[] ProductUser { get; set; }
}
