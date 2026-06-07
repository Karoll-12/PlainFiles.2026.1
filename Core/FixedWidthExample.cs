namespace Core;

public class FixedWidthExample
{
    public void Write(string path, IEnumerable<FixedWidthRecord> records)
    {
        File.WriteAllLines(path, records.Select(x => x.ToString()));
    }

    public List<FixedWidthRecord> Read(string path)
    {
        var lines = File.ReadAllLines(path);
        return lines.Select(FixedWidthRecord.Parse).ToList();
    }
}
