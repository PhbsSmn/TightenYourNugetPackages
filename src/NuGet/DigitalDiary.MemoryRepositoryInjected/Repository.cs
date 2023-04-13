using System.Net.Mime;

namespace DigitalDiary.MemoryRepository;

/// <summary>
/// Memory repository
/// </summary>
public class Repository : IDigitalDiaryRepository
{
    private readonly Dictionary<DateTime, string> _messages;

    /// <summary>
    /// The memory repository project name
    /// </summary>
    public string RepositoryProjectName => $"DigitalDiary.MemoryRepositoryInjected: {typeof(Repository).Assembly.GetName().Version}";

    /// <summary>
    /// Create an instance of <see cref="Repository"/>
    /// </summary>
    public Repository()
    {
        _messages = new Dictionary<DateTime, string>();
    }

    /// <summary>
    /// Write to repo
    /// </summary>
    /// <param name="message"></param>
    public void Write(string message)
    {
        _messages.Add(DateTime.UtcNow, message);
    }

    /// <summary>
    /// Read from repo
    /// </summary>
    /// <returns></returns>
    public IEnumerable<(int Id, DateTime TimeStamp, string Value)> Read()
    {
        return _messages.OrderBy(m => m.Key)
            .Select((message, index) => new ValueTuple<int, DateTime, string>(index, message.Key, message.Value));
    }
}