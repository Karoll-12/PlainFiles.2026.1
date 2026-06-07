using System.Globalization;

namespace Core;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string City { get; set; } = null!;
    public decimal Balance { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string[] ToFields() =>
        new[] { Id.ToString(), FirstName, LastName, Phone, City, Balance.ToString("F2", CultureInfo.InvariantCulture) };

    public static Person FromFields(string[] fields) => new()
    {
        Id = int.Parse(fields[0]),
        FirstName = fields[1],
        LastName = fields[2],
        Phone = fields[3],
        City = fields[4],
        Balance = decimal.Parse(fields[5], CultureInfo.InvariantCulture)
    };
}