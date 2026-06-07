namespace Core;

public class SimpleTextFile
{
    private readonly string _path;

    public SimpleTextFile(string path)
    {
        _path = path;
    }

    public void WriteLines(string[] lines)
    {
        EnsureDirectory();
        File.WriteAllLines(_path, lines);
    }

    public string[] ReadLines()
    {
        if (!File.Exists(_path))
        {
            EnsureDirectory();
            File.WriteAllText(_path, string.Empty);
            return Array.Empty<string>();
        }
        return File.ReadAllLines(_path);
    }

    private void EnsureDirectory()
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }
}
