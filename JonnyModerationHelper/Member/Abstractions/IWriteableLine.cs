namespace JonnyModerationHelper.Member.Abstractions;

public interface IWriteableLine : IPartialLine
{
    public long Version { get; protected set; }
    public string       Reason      { get; set; }
}