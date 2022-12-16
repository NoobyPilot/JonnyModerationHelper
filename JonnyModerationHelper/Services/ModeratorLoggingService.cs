using JonnyModerationHelper.Member;
using JonnyModerationHelper.Member.Objects;
using JonnyModerationHelper.Services.Abstractions;

namespace JonnyModerationHelper.Services;

public class ModeratorLoggingService : IModeratorLoggingService
{

    private readonly IModerationDatabaseService _moderationDatabaseService;

    public ModeratorLoggingService(IModerationDatabaseService moderationDatabaseService)
    {
        _moderationDatabaseService = moderationDatabaseService;
    }
    
    public void LogInfoAccess(ulong guildId, ulong moderatorId, ulong userId)
    {
        _moderationDatabaseService.WriteLine(guildId, new WriteableLine(moderatorId, InfoLineType.Access, moderatorId, userId));
    }

    public void LogWarning(ulong    guildId, ulong moderatorId, ulong userId, string    reason)
    {
        throw new NotImplementedException();
    }

    public void LogDelete(ulong     guildId, ulong moderatorId, ulong userId, string    channelName, string reason)
    {
        throw new NotImplementedException();
    }

    public void LogMute(ulong       guildId, ulong moderatorId, ulong userId, TimeSpan  duration,    string reason)
    {
        throw new NotImplementedException();
    }

    public void LogKick(ulong       guildId, ulong moderatorId, ulong userId, string    reason)
    {
        throw new NotImplementedException();
    }

    public void LogBan(ulong        guildId, ulong moderatorId, ulong userId, TimeSpan? duration, string reason)
    {
        throw new NotImplementedException();
    }
}