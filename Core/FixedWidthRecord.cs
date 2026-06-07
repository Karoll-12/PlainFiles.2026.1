namespace Core;

public class FixedWidthRecord
{
    // Code: 5 characters
    public string Code { get; set; } = null!;

    // Description: 40 characters
    public string Description { get; set; } = null!;

    // Category: 15 characters
    public string Category { get; set; } = null!;

    // Quantity: 5 characters
    public int Quantity { get; set; }

    // Price: 12 characters
    public decimal Price { get; set; }

    public decimal Value => Price * Quantity;

    public override string ToString()
    {
        return $"{Code.PadRight(5)}{Description.PadRight(40)}{Category.PadRight(15)}{Quantity.ToString().PadLeft(5)}{Price.ToString("F2").PadLeft(12)}";
    }

    public static FixedWidthRecord Parse(string line)
    {
        return new FixedWidthRecord
        {
            Code = line.Substring(0, 5).Trim(),
            Description = line.Substring(5, 40).Trim(),
            Category = line.Substring(45, 15).Trim(),
            Quantity = int.Parse(line.Substring(60, 5)),
            Price = decimal.Parse(line.Substring(65, 12))
        };
    }
}
