using System.Drawing;
using JonnyModerationHelper.Member;
using JonnyModerationHelper.Member.Abstractions;
using JonnyModerationHelper.Member.Objects;
using JonnyModerationHelper.Services.Abstractions;
using JonnyModerationHelper.Services.Discord.Abstractions;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;
using Humanizer;

namespace JonnyModerationHelper.Services.Discord;

public class DiscordMemberModerationService : IDiscordMemberModerationService
{

    private IMemberModerationService                _moderationService;
    private IDiscordRestUserAPI                     _userApi;
    private ILogger<DiscordMemberModerationService> _logger;

    public DiscordMemberModerationService(IMemberModerationService moderationService, IDiscordRestUserAPI userApi, ILogger<DiscordMemberModerationService> logger)
    {
        _moderationService = moderationService;
        _userApi = userApi;
        _logger = logger;
    }
    
    public async Task<Embed> GetMemberInfo(ulong guildId, ulong userId)
    {
        _logger.LogInformation("Looking up member info");
        var lines = _moderationService.GetMemberInfo(guildId, new LineQuerySelector() { UserId = userId, Limit = 25, LineType = InfoLineType.Note});
        _logger.LogInformation("Got member info, building fields");
        var embedFields = lines.Select(BuildEmbedField).ToArray();
        _logger.LogInformation("Built fields, now looking up the user");
        var user = await _userApi.GetUserAsync(new Snowflake(userId));
        _logger.LogInformation("Got user, now building and returning embed");
        return new Embed($"{user.Entity.Username}#{user.Entity.Discriminator}", Colour: Color.DarkRed, Fields: embedFields, Timestamp: DateTimeOffset.Now);
    }

    private EmbedField BuildEmbedField(ILine line)
    {
        var typeId = $"{line.LineType.ToString()} ({line.Id})";
        typeId = typeId.PadRight(11, ' ');
        var moderator = $"<@{line.ModeratorId}>";
        string timeVersion;
        if (line.EditedAt == null)
        {
            timeVersion = $"{line.CreatedAt.ToShortDateString()} {line.CreatedAt.ToShortTimeString()}";
        }
        else
        {
            timeVersion = $"{line.EditedAt.Value.ToShortDateString()} {line.EditedAt.Value.ToShortTimeString()} ({line.Version})";
        }

        var channel = $"#{line.Channel ?? "NULL"}";
        channel = channel.PadRight(20, ' ');
        var duration = (line.Duration ?? TimeSpan.Zero).Duration().Humanize();
        return new EmbedField(typeId + " | " + moderator + " | " + timeVersion, "**" + channel + " | " + duration + "**\n" + $"{line.Reason}");
    }


    public async Task<bool> WriteInfoLine(ulong guildId, ulong userId, ulong moderatorId, string reason)
    {
        _logger.LogDebug("Forwarding write");
        try
        {
            await _moderationService.WriteLine(guildId, new WriteableLine(moderatorId, InfoLineType.Note, moderatorId,
                                                                          userId,
                                                                          reason: reason));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception was thrown while trying to write a line to the database:\n{e}");
            return false;
        }
    }

    public async Task<bool> WriteWarningLine(ulong guildId, ulong userId, ulong moderatorId, string reason)
    {
        _logger.LogDebug("Forwarding write");
        try
        {
            await _moderationService.WriteLine(guildId,
                                               new WriteableLine(moderatorId, InfoLineType.Warn, moderatorId, userId,
                                                                 reason: reason));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception was thrown while trying to write a line to the database:\n{e}");
            return false;
        }
    }

    public async Task<bool> WriteDeleteLine(ulong  guildId, ulong userId, ulong moderatorId, string channelName, string   reason)
    {
        _logger.LogDebug("Forwarding write");
        try
        {
            await _moderationService.WriteLine(guildId,
                                               new WriteableLine(moderatorId, InfoLineType.Delete, moderatorId, userId,
                                                                 reason: reason));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception was thrown while trying to write a line to the database:\n{e}");
            return false;
        }
    }

    public async Task<bool> WriteMuteLine(ulong    guildId, ulong userId, ulong moderatorId, string reason,      TimeSpan duration)
    {
        _logger.LogDebug("Forwarding write");
        try
        {
            await _moderationService.WriteLine(guildId,
                                               new WriteableLine(moderatorId, InfoLineType.Mute, moderatorId, userId,
                                                                 reason: reason));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception was thrown while trying to write a line to the database:\n{e}");
            return false;
        }
    }

    public async Task<bool>     WriteKickLine(ulong guildId, ulong userId, ulong moderatorId, string reason)
    {
        _logger.LogDebug("Forwarding write");
        try
        {
            await _moderationService.WriteLine(guildId,
                                               new WriteableLine(moderatorId, InfoLineType.Kick, moderatorId, userId,
                                                                 reason: reason));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception was thrown while trying to write a line to the database:\n{e}");
            return false;
        }
    }

    public async Task<bool> WriteBanLine(ulong guildId, ulong userId, ulong moderatorId, string reason, TimeSpan? duration)
    {
        _logger.LogDebug("Forwarding write");
        try
        {
            await _moderationService.WriteLine(guildId,
                                               new WriteableLine(moderatorId, InfoLineType.Ban, moderatorId, userId,
                                                                 reason: reason));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception was thrown while trying to write a line to the database:\n{e}");
            return false;
        }
    }
}