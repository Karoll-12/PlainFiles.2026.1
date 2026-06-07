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

    /// <summary>Log an event without a specific user (e.g. login attempts).</summary>
    public void WriteLog(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("s");
        _writer.WriteLine($"{timestamp} [{level.ToUpper()}] {message}");
    }

    /// <summary>Log an event associated with an authenticated user.</summary>
    public void WriteLog(string level, string username, string message)
    {
        var timestamp = DateTime.Now.ToString("s");
        _writer.WriteLine($"{timestamp} [{level.ToUpper()}] [user:{username}] {message}");
    }

    public void Dispose() => _writer.Dispose();
}
