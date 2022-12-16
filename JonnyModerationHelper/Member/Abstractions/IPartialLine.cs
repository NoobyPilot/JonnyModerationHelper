namespace JonnyModerationHelper.Member.Abstractions;

public interface IPartialLine
{
    public long         Id          { get; }
    public ulong        CreatedBy   { get; }
    public DateTime     CreatedAt   { get; }
    public InfoLineType LineType    { get; }
    public ulong        ModeratorId { get; }
    public ulong        UserId      { get; }
}