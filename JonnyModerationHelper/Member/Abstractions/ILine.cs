namespace JonnyModerationHelper.Member.Abstractions;

public interface ILine : IWriteableLine
{
    public DateTime? EditedAt { get; protected set; }
    public ulong?    EditedBy { get; protected set; }
    public TimeSpan? Duration { get; }
    public string?   Channel  { get; }
}