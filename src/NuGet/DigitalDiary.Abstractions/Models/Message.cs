namespace DigitalDiary.Models;

/// <summary>
/// The message written in the digital diary
/// </summary>
/// <param name="Id">The id</param>
/// <param name="Value">The message</param>
/// <param name="Timestamp">The time when the message was written</param>
public record Message(int Id, string Value, DateTime Timestamp);