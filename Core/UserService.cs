namespace Core;

/// <summary>
/// Handles authentication against Users.txt, enforces 3-attempt lockout,
/// and persists changes back to the file.
/// </summary>
public class UserService
{
    private const int MaxAttempts = 3;
    private readonly string _usersPath;
    private readonly LogWriter _log;
    private List<UserAccount> _users;

    public UserService(string usersPath, LogWriter log)
    {
        _usersPath = usersPath;
        _log = log;
        _users = Load();
    }

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>
    /// Interactively prompts for credentials until login succeeds or the
    /// user is locked out. Returns the username on success, null if the
    /// account was locked.
    /// </summary>
    public string? Login()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;

        var account = _users.FirstOrDefault(
            u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (account == null)
        {
            _log.WriteLog("WARN", $"Login attempt for unknown user '{username}'.");
            Console.WriteLine("User not found.");
            return null;
        }

        if (!account.IsActive)
        {
            _log.WriteLog("WARN", $"Login attempt for locked user '{username}'.");
            Console.WriteLine("This account is locked. Contact an administrator.");
            return null;
        }

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            Console.Write("Password: ");
            var password = ReadPassword();

            if (password == account.Password)
            {
                _log.WriteLog("INFO", $"User '{username}' logged in successfully.");
                Console.WriteLine($"\nWelcome, {username}!\n");
                return username;
            }

            int remaining = MaxAttempts - attempt;
            if (remaining > 0)
            {
                Console.WriteLine($"Wrong password. {remaining} attempt(s) remaining.");
                _log.WriteLog("WARN", $"Failed login attempt {attempt}/{MaxAttempts} for '{username}'.");
            }
            else
            {
                account.IsActive = false;
                Save();
                _log.WriteLog("ERROR", $"User '{username}' locked after {MaxAttempts} failed attempts.");
                Console.WriteLine("Too many failed attempts. Your account has been locked.");
                return null;
            }
        }

        return null; // unreachable but satisfies compiler
    }

    // ---------------------------------------------------------------
    // Private helpers
    // ---------------------------------------------------------------

    private List<UserAccount> Load()
    {
        var file = new SimpleTextFile(_usersPath);
        return file.ReadLines()
                   .Where(l => !string.IsNullOrWhiteSpace(l))
                   .Select(l => UserAccount.FromFields(l.Split(',')))
                   .ToList();
    }

    private void Save()
    {
        var file = new SimpleTextFile(_usersPath);
        file.WriteLines(_users.Select(u => string.Join(",", u.ToFields())).ToArray());
    }

    /// <summary>Reads a password from the console without echoing characters.</summary>
    private static string ReadPassword()
    {
        var sb = new System.Text.StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                sb.Append(key.KeyChar);
            }
        }
        Console.WriteLine();
        return sb.ToString();
    }
}
