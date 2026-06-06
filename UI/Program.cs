using Care;

var textFile = new SimpleTextFile("c:\\tmp\\animals.txt");
var lines = textFile.ReadLines();
var list = lines.ToList();
var option = string.Empty;

do
{
    option = Menu();
    switch (option)
    {
        case "1":
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            break;
        case "2":
            Console.Write("Enter a new line: ");
            var newLine = Console.ReadLine();
            list.Add(newLine!);
            break;
        case "3":
            Console.Write("Enter a new line: ");
            var LineToRemove = Console.ReadLine();
            list.Remove(LineToRemove!);
            break;
        case "4":
            textFile.WriteLines(list.ToArray());
            Console.WriteLine("Changes saved.");
            break;
        default:
            break;
    }
} while (option != "0");
textFile.WriteLines(list.ToArray());
Console.WriteLine("Changes saved.");

string Menu()
{
    Console.WriteLine("Menu:");
    Console.WriteLine("1. Show lines");
    Console.WriteLine("2. Add line");
    Console.WriteLine("3. Remove ocurrence");
    Console.WriteLine("4. Remove all ocurrences");
    Console.WriteLine("5. Sort");
    Console.WriteLine("6. Save changes");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");
    return Console.ReadLine() ?? string.Empty;
}