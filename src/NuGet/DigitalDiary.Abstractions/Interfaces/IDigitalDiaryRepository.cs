namespace DigitalDiary;

/// <summary>
/// The digital diary repository interface
/// </summary>
public interface IDigitalDiaryRepository
{
    /// <summary>
    /// The memory repository project name
    /// </summary>
    string RepositoryProjectName { get; }

    /// <summary>
    /// Writes a message to the digital diary
    /// </summary>
    /// <param name="message">The message</param>
    void Write(string message);

    /// <summary>
    /// Reads all the message from the digital diary repo
    /// </summary>
    /// <returns>An <see cref="IEnumerable{Message}"/></returns>
    IEnumerable<(int Id, DateTime TimeStamp, string Value)> Read();
}