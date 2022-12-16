namespace JonnyModerationHelper.Services.Abstractions;

public interface IModeratorLoggingService
{

    public void LogInfoAccess(ulong guildId, ulong moderatorId, ulong userId);
    public void LogWarning(ulong    guildId, ulong moderatorId, ulong userId, string    reason);
    public void LogDelete(ulong     guildId, ulong moderatorId, ulong userId, string    channelName, string reason);
    public void LogMute(ulong       guildId, ulong moderatorId, ulong userId, TimeSpan  duration,    string reason);
    public void LogKick(ulong       guildId, ulong moderatorId, ulong userId, string    reason);
    public void LogBan(ulong        guildId, ulong moderatorId, ulong userId, TimeSpan? duration, string   reason);

}