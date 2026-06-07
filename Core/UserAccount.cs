namespace Core;

public class UserAccount
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; }

    public string[] ToFields() =>
        new[] { Username, Password, IsActive.ToString().ToLower() };

    public static UserAccount FromFields(string[] fields) => new()
    {
        Username = fields[0],
        Password = fields[1],
        IsActive = bool.Parse(fields[2])
    };
}
