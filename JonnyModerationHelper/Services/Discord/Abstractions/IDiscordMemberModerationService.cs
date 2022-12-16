using Remora.Discord.API.Objects;

namespace JonnyModerationHelper.Services.Discord.Abstractions;

public interface IDiscordMemberModerationService
{
    public Task<Embed> GetMemberInfo(ulong    guildId, ulong userId);
    public Task<bool>  WriteInfoLine(ulong    guildId, ulong userId, ulong moderatorId, string reason);
    public Task<bool>  WriteWarningLine(ulong guildId, ulong userId, ulong moderatorId, string reason);

    public Task<bool> WriteDeleteLine(ulong  guildId, ulong userId, ulong moderatorId, string channelName,
                                      string reason);

    public Task<bool> WriteMuteLine(ulong guildId, ulong userId, ulong moderatorId, string reason, TimeSpan  duration);
    public Task<bool> WriteKickLine(ulong guildId, ulong userId, ulong moderatorId, string reason);
    public Task<bool> WriteBanLine(ulong guildId, ulong userId, ulong moderatorId, string reason, TimeSpan? duration);
}