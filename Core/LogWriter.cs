namespace Core;

public class LogWriter : IDisposable
{
    private readonly StreamWriter _writer;

    public LogWriter(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        _writer = new StreamWriter(path, append: true) { AutoFlush = true };
    }

    public void WriteLog(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("s");
        _writer.WriteLine($"{timestamp} [{level.ToUpper()}] {message}");
    }

    public void WriteLog(string level, string username, string message)
    {
        var timestamp = DateTime.Now.ToString("s");
        _writer.WriteLine($"{timestamp} [{level.ToUpper()}] [user:{username}] {message}");
    }

    public void Dispose() => _writer.Dispose();
}
