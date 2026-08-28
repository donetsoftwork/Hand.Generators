namespace GenerateConvertTests.Supports;

public class Product(int productId, string productName)
{
    public int ProductId { get; } = productId;
    public string ProductName { get; } = productName;
    public MyColor ProductColor { get; set; }

    public User ProductUser { get; set; }
}
