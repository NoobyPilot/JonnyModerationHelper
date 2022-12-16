using JonnyModerationHelper.Member.Abstractions;

namespace JonnyModerationHelper.Member.Objects;

public class PartialLine : IPartialLine
{
    public long         Id          { get; }
    public ulong        CreatedBy   { get; }
    public DateTime     CreatedAt   { get; }
    public InfoLineType LineType    { get; }
    public ulong        ModeratorId { get; }
    public ulong        UserId      { get; }

    public PartialLine(long id, ulong createdBy, DateTime createdAt, InfoLineType lineType, ulong moderatorId, ulong userId)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        LineType = lineType;
        ModeratorId = moderatorId;
        UserId = userId;
    }
    
}