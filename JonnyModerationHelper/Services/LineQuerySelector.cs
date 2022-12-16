using JonnyModerationHelper.Member;

namespace JonnyModerationHelper.Services;

public struct LineQuerySelector
{
    public ulong?        UserId;
    public ulong?        ModeratorId;
    public bool?         ExactLineTypeMatch;
    public InfoLineType? LineType;
    public DateTime?     CreatedSince;
    public int?          Limit;
    public int?          Offset;
}