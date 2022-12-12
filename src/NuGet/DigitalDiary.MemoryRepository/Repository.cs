namespace DigitalDiary.MemoryRepository;

/// <summary>
/// Memory repository
/// </summary>
public class Repository : IDigitalDiaryRepository
{
    private readonly Dictionary<DateTime, string> _messages;

    private Version CurrentVersion { get; }

    /// <summary>
    /// The memory repository project name
    /// </summary>
    public string RepositoryProjectName => $"DigitalDiary.MemoryRepository: {CurrentVersion}";

    /// <summary>
    /// Create an instance of <see cref="Repository"/>
    /// </summary>
    public Repository()
    {
        _messages = new Dictionary<DateTime, string>();
        CurrentVersion = typeof(Repository).Assembly.GetName().Version ?? new Version(1, 0, 0);
    }

    /// <summary>
    /// Write to repo
    /// </summary>
    /// <param name="message"></param>
    public void Write(string message)
    {
        switch (CurrentVersion)
        {
            case { Major: 1, Minor: 0, Build: 0 }:
                throw new Exception("Just doesn't work");
            default:
                _messages.Add(DateTime.UtcNow, message);
                break;
        }
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