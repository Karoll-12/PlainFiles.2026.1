using Core;

// ── Paths ──────────────────────────────────────────────────────────
const string BasePath = "c:\\tmp";
const string UsersFile = $"{BasePath}\\Users.txt";
const string PeopleFile = $"{BasePath}\\People.txt";
const string LogFile = $"{BasePath}\\log.txt";

// ── Bootstrap ──────────────────────────────────────────────────────
using var log = new LogWriter(LogFile);
log.WriteLog("INFO", "Application started.");

// ── Authentication ─────────────────────────────────────────────────
EnsureUsersFile(UsersFile);

var userService = new UserService(UsersFile, log);

Console.WriteLine("╔══════════════════════════════╗");
Console.WriteLine("║     Person Manager v1.0      ║");
Console.WriteLine("╚══════════════════════════════╝");
Console.WriteLine();

var currentUser = userService.Login();
if (currentUser == null)
{
    log.WriteLog("INFO", "Application ended (login failed).");
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
    return;
}

// ── Main loop ──────────────────────────────────────────────────────
var personService = new PersonService(PeopleFile, log, currentUser);

string option;
do
{
    option = ShowMenu();
    try
    {
        switch (option)
        {
            case "1": personService.ListAll(); break;
            case "2": personService.Create(); break;
            case "3": personService.Update(); break;
            case "4": personService.Delete(); break;
            case "5": personService.Report(); break;
            case "0":
                log.WriteLog("INFO", currentUser, "User logged out.");
                Console.WriteLine("\nGoodbye!");
                break;
            default:
                Console.WriteLine("  Invalid option. Please try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        log.WriteLog("ERROR", currentUser, $"Unhandled exception: {ex.Message}");
        Console.WriteLine($"\n  Error: {ex.Message}");
    }

    if (option != "0")
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

} while (option != "0");

log.WriteLog("INFO", "Application ended.");

// ── Helpers ────────────────────────────────────────────────────────
static string ShowMenu()
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════╗");
    Console.WriteLine("║     Person Manager v1.0      ║");
    Console.WriteLine("╠══════════════════════════════╣");
    Console.WriteLine("║  1. List people              ║");
    Console.WriteLine("║  2. Add person               ║");
    Console.WriteLine("║  3. Edit person              ║");
    Console.WriteLine("║  4. Delete person            ║");
    Console.WriteLine("║  5. Report by city           ║");
    Console.WriteLine("║  0. Exit                     ║");
    Console.WriteLine("╚══════════════════════════════╝");
    Console.Write("Choose an option: ");
    return Console.ReadLine()?.Trim() ?? string.Empty;
}

static void EnsureUsersFile(string path)
{
    if (!File.Exists(path))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        // Default seed users
        File.WriteAllLines(path, new[]
        {
            "jzuluaga,P@ssw0rd123!,true",
            "mbedoya,S0yS3gur02025*,false"
        });
    }
}
