using System;
using System.IO;

namespace Care;

public class SimpleTextFile
{
    private readonly string _Path;

    public SimpleTextFile(string path)
    {
        _Path = path;
    }

    public void WriteLines(string[] lines)
    {
        var directory = Path.GetDirectoryName(_Path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllLines(_Path, lines);
    }


    public string[] ReadLines()
    {
        if (!File.Exists(_Path))
        {
            var directory = Path.GetDirectoryName(_Path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(_Path, string.Empty);
            return Array.Empty<string>();
        }

        return File.ReadAllLines(_Path);
    }

}
