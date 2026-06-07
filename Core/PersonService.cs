using System.Globalization;
using System.Text.RegularExpressions;

namespace Core;

/// <summary>
/// Manages the list of Person records stored in a CSV-like plain text file.
/// Every mutating operation writes to the log with the acting username.
/// </summary>
public class PersonService
{
    private readonly string _dataPath;
    private readonly LogWriter _log;
    private readonly string _username;
    private List<Person> _people;

    public PersonService(string dataPath, LogWriter log, string username)
    {
        _dataPath = dataPath;
        _log = log;
        _username = username;
        _people = Load();
    }

    // ================================================================
    // CREATE
    // ================================================================
    public void Create()
    {
        Console.WriteLine("\n--- Add Person ---");

        // ID
        int id;
        while (true)
        {
            Console.Write("ID (number): ");
            var raw = Console.ReadLine()?.Trim() ?? "";
            if (!int.TryParse(raw, out id) || id <= 0)
            { Console.WriteLine("  ID must be a positive integer."); continue; }
            if (_people.Any(p => p.Id == id))
            { Console.WriteLine("  That ID already exists. Choose another."); continue; }
            break;
        }

        var firstName = ReadRequired("First name");
        var lastName = ReadRequired("Last name");
        var phone = ReadPhone();
        var city = ReadRequired("City");
        var balance = ReadPositiveDecimal("Balance");

        var person = new Person
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            City = city,
            Balance = balance
        };

        _people.Add(person);
        Save();
        _log.WriteLog("INFO", _username, $"Person created: ID={id}, Name={person.FullName}");
        Console.WriteLine("  Person added successfully.");
    }

    // ================================================================
    // READ (list all)
    // ================================================================
    public void ListAll()
    {
        Console.WriteLine("\n--- Person List ---");
        if (!_people.Any()) { Console.WriteLine("  No records found."); return; }

        PrintHeader();
        foreach (var p in _people.OrderBy(p => p.Id))
            PrintRow(p);

        _log.WriteLog("INFO", _username, "Listed all persons.");
    }

    // ================================================================
    // UPDATE
    // ================================================================
    public void Update()
    {
        Console.WriteLine("\n--- Edit Person ---");
        var person = FindById("Enter ID to edit");
        if (person == null) return;

        Console.WriteLine($"  Editing: {person.FullName} — press ENTER to keep current value.\n");

        var firstName = ReadOptional($"First name [{person.FirstName}]");
        var lastName = ReadOptional($"Last name  [{person.LastName}]");
        var phone = ReadOptionalPhone($"Phone      [{person.Phone}]", person.Phone);
        var city = ReadOptional($"City       [{person.City}]");
        var balanceRaw = ReadOptionalDecimal($"Balance    [{person.Balance:F2}]", person.Balance);

        if (!string.IsNullOrEmpty(firstName)) person.FirstName = firstName;
        if (!string.IsNullOrEmpty(lastName)) person.LastName = lastName;
        person.Phone = phone;
        if (!string.IsNullOrEmpty(city)) person.City = city;
        person.Balance = balanceRaw;

        Save();
        _log.WriteLog("INFO", _username, $"Person updated: ID={person.Id}, Name={person.FullName}");
        Console.WriteLine("  Person updated successfully.");
    }

    // ================================================================
    // DELETE
    // ================================================================
    public void Delete()
    {
        Console.WriteLine("\n--- Delete Person ---");
        var person = FindById("Enter ID to delete");
        if (person == null) return;

        Console.WriteLine("\n  Record to delete:");
        PrintHeader();
        PrintRow(person);

        Console.Write("\n  Are you sure you want to delete this person? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();
        if (confirm != "y")
        { Console.WriteLine("  Delete cancelled."); return; }

        _people.Remove(person);
        Save();
        _log.WriteLog("INFO", _username, $"Person deleted: ID={person.Id}, Name={person.FullName}");
        Console.WriteLine("  Person deleted successfully.");
    }

    // ================================================================
    // REPORT — subtotals by city
    // ================================================================
    public void Report()
    {
        Console.WriteLine("\n--- Balance Report by City ---\n");
        if (!_people.Any()) { Console.WriteLine("  No records found."); return; }

        var groups = _people
            .OrderBy(p => p.City)
            .ThenBy(p => p.Id)
            .GroupBy(p => p.City);

        decimal grandTotal = 0;

        foreach (var group in groups)
        {
            Console.WriteLine($"City: {group.Key}");
            Console.WriteLine($"{"ID",-6} {"First Name",-20} {"Last Name",-20} {"Balance",15}");
            Console.WriteLine($"{"—",-6} {"—",-20} {"—",-20} {"—",15}");

            decimal cityTotal = 0;
            foreach (var p in group)
            {
                Console.WriteLine($"{p.Id,-6} {p.FirstName,-20} {p.LastName,-20} {p.Balance.ToString("N2", System.Globalization.CultureInfo.InvariantCulture),15}");
                cityTotal += p.Balance;
            }

            Console.WriteLine($"{"",48} {"=======",15}");
            Console.WriteLine($"Total: {group.Key,-41} {cityTotal.ToString("N2", System.Globalization.CultureInfo.InvariantCulture),15}");
            Console.WriteLine();
            grandTotal += cityTotal;
        }

        Console.WriteLine($"{"",48} {"=======",15}");
        Console.WriteLine($"{"Total General:",-48} {grandTotal.ToString("N2", System.Globalization.CultureInfo.InvariantCulture),15}");

        _log.WriteLog("INFO", _username, $"Report by city generated. Grand total: {grandTotal:N2}");
    }

    // ================================================================
    // Private helpers — persistence
    // ================================================================
    private List<Person> Load()
    {
        var file = new SimpleTextFile(_dataPath);
        return file.ReadLines()
                   .Where(l => !string.IsNullOrWhiteSpace(l))
                   .Select(l => Person.FromFields(l.Split(',')))
                   .ToList();
    }

    private void Save()
    {
        var file = new SimpleTextFile(_dataPath);
        file.WriteLines(_people.Select(p => string.Join(",", p.ToFields())).ToArray());
    }

    // ================================================================
    // Private helpers — console input
    // ================================================================
    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            var val = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(val)) return val;
            Console.WriteLine($"  {prompt} is required.");
        }
    }

    private static string ReadPhone()
    {
        while (true)
        {
            Console.Write("  Phone: ");
            var val = Console.ReadLine()?.Trim() ?? "";
            if (IsValidPhone(val)) return val;
            Console.WriteLine("  Invalid phone. Use 7–15 digits, optionally starting with + (e.g. +573001234567).");
        }
    }

    private static string ReadOptionalPhone(string prompt, string current)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            var val = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(val)) return current;
            if (IsValidPhone(val)) return val;
            Console.WriteLine("  Invalid phone. Use 7–15 digits, optionally starting with +.");
        }
    }

    private static bool IsValidPhone(string phone) =>
        Regex.IsMatch(phone, @"^\+?[0-9]{7,15}$");

    private static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            var raw = Console.ReadLine()?.Trim() ?? "";
            if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture, out var val)
                && val >= 0)
                return val;
            Console.WriteLine("  Balance must be a non-negative number.");
        }
    }

    private static decimal ReadOptionalDecimal(string prompt, decimal current)
    {
        while (true)
        {
            Console.Write($"  {prompt}: ");
            var raw = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(raw)) return current;
            if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture, out var val)
                && val >= 0)
                return val;
            Console.WriteLine("  Balance must be a non-negative number.");
        }
    }

    private static string ReadOptional(string prompt)
    {
        Console.Write($"  {prompt}: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    private Person? FindById(string prompt)
    {
        Console.Write($"  {prompt}: ");
        var raw = Console.ReadLine()?.Trim() ?? "";
        if (!int.TryParse(raw, out var id))
        { Console.WriteLine("  Invalid ID."); return null; }

        var person = _people.FirstOrDefault(p => p.Id == id);
        if (person == null)
            Console.WriteLine($"  Person with ID {id} not found.");
        return person;
    }

    // ================================================================
    // Private helpers — display
    // ================================================================
    private static void PrintHeader()
    {
        Console.WriteLine($"\n{"ID",-6} {"First Name",-20} {"Last Name",-20} {"Phone",-16} {"City",-15} {"Balance",12}");
        Console.WriteLine($"{"—",-6} {"—",-20} {"—",-20} {"—",-16} {"—",-15} {"—",12}");
    }

    private static void PrintRow(Person p) =>
        Console.WriteLine($"{p.Id,-6} {p.FirstName,-20} {p.LastName,-20} {p.Phone,-16} {p.City,-15} {p.Balance.ToString("N2", System.Globalization.CultureInfo.InvariantCulture),12}");
}