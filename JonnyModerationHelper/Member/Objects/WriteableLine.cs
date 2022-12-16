using JonnyModerationHelper.Member.Abstractions;

namespace JonnyModerationHelper.Member.Objects;

public class WriteableLine : IWriteableLine
{
    public long         Id          { get; }
    public ulong        CreatedBy   { get; }
    public DateTime     CreatedAt   { get; }
    public InfoLineType LineType    { get; }
    public ulong        ModeratorId { get; }
    public ulong        UserId      { get; }
    public long         Version     { get; set; }
    public string       Reason      { get; set; }

    public WriteableLine(ulong  createdBy, InfoLineType lineType, ulong moderatorId, ulong userId, long version = 1,
                         string reason = "")
    {
        Id = 0;
        CreatedBy = createdBy;
        CreatedAt = DateTime.Now;
        LineType = lineType;
        ModeratorId = moderatorId;
        UserId = userId;
        Version = version;
        Reason = reason;
    }
    
}