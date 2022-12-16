using JonnyModerationHelper.Member.Abstractions;

namespace JonnyModerationHelper.Member.Objects;

public class Line : ILine
{
    public long         Id          { get; }
    public ulong        CreatedBy   { get; }
    public DateTime     CreatedAt   { get; }
    public InfoLineType LineType    { get; }
    public ulong        ModeratorId { get; }
    public ulong        UserId      { get; }
    public long         Version     { get; set; }
    public DateTime?    EditedAt    { get; set; }
    public ulong?       EditedBy    { get; set; }
    public string       Reason      { get; set; }
    public TimeSpan?    Duration    { get; }
    public string?      Channel     { get; }

    public Line(long id, ulong createdBy, DateTime createdAt, InfoLineType lineType, ulong moderatorId, ulong userId, long version, DateTime? editedAt, ulong? editedBy, string reason, TimeSpan? duration, string? channel)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        LineType = lineType;
        ModeratorId = moderatorId;
        UserId = userId;
        Version = version;
        EditedAt = editedAt;
        EditedBy = editedBy;
        Reason = reason;
        Duration = duration;
        Channel = channel;
    }
}