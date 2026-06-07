public class ManualCsvHelper
{
    public void WriteCSV(string path, List<string[]> records)
    {
        using var sw = new StreamWriter(path);
        foreach (var field in records)
        {
            var line = string.Join(",", field);
            sw.WriteLine(line);
        }
    }

    public List<string[]> ReadCSV(string path)
    {
        var result = new List<string[]>();
        using var sr = new StreamReader(path);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var fields = line.Split(',');
            result.Add(fields);
        }
        return result;
    }
}