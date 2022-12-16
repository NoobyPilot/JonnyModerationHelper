using System.ComponentModel;
using JonnyModerationHelper.Services.Abstractions;
using JonnyModerationHelper.Services.Discord.Abstractions;
using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace JonnyModerationHelper.Commands;

[Group("user")]
public class UserCommands : CommandGroup
{
    private readonly FeedbackService                 _feedbackService;
    private readonly IDiscordMemberModerationService _moderationService;
    private readonly IModeratorLoggingService        _moderatorLogging;
    private readonly ICommandContext                 _context;
    private readonly ILogger<UserCommands>           _logger;

    public UserCommands(FeedbackService                 feedbackService,   ICommandContext          context,
                        IDiscordMemberModerationService moderationService, IModeratorLoggingService moderatorLogging,
                        ILogger<UserCommands>           logger)
    {
        _feedbackService = feedbackService;
        _context = context;
        _moderationService = moderationService;
        _logger = logger;
        _moderatorLogging = moderatorLogging;
    }

    [Command("info")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Get all info belonging to a user")]
    [Ephemeral]
    public async Task<IResult> GetUserInfo([Description("The moderator executing (aka you)")] IUser moderator,
                                           [Description("The user to get the info for")]      IUser target)
    {
        _logger.LogInformation("Received User Lookup Command");
        if (_context is not InteractionContext interactionContext)
        {
            _logger.LogInformation("Not an interaction context");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got interaction context");
        var possibleGuild = interactionContext.Interaction.GuildID;
        if (!possibleGuild.IsDefined(out var guildId))
        {
            _logger.LogInformation("Not started from within a guild");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got guild id");
        _logger.LogInformation("Logging the lookup access");
        _moderatorLogging.LogInfoAccess(guildId.Value, moderator.ID.Value, target.ID.Value);
        _logger.LogInformation("Creating lookup embed");
        var embed = await _moderationService.GetMemberInfo(guildId.Value, target.ID.Value);
        _logger.LogInformation("Sending embed");
        return (Result)await _feedbackService.SendContextualEmbedAsync(embed);
    }

    [Command("note")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Add a note to the target user")]
    [Ephemeral]
    public async Task<IResult> AddUserNote([Description("The moderator executing (aka you)")] IUser  moderator,
                                           [Description("The user to add the note to")]       IUser  target,
                                           [Description("The note to be displayed")]          string message)
    {
        _logger.LogInformation("Received User Note Add Command");
        if (_context is not InteractionContext interactionContext)
        {
            _logger.LogInformation("Not an interaction context");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got interaction context");
        var possibleGuild = interactionContext.Interaction.GuildID;
        if (!possibleGuild.IsDefined(out var guildId))
        {
            _logger.LogInformation("Not started from within a guild");
            return Result.FromSuccess();
        }

        if (await _moderationService.WriteInfoLine(guildId.Value, target.ID.Value, moderator.ID.Value, message))
            return (Result)await _feedbackService.SendContextualSuccessAsync("Successfully added new line");
        return (Result)await _feedbackService.SendContextualErrorAsync("Could not add new line");
    }

    [Command("mute")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Add a mute to the target user")]
    [Ephemeral]
    public async Task<IResult> AddUserMute([Description("The moderator executing (aka you)")] IUser  moderator,
                                           [Description("The user to mute")]                  IUser  target,
                                           [Description("The reason for the mute")]           string reason,
                                           [Description("The duration of the mute")]          string muteDuration)
    {
        _logger.LogInformation("Received User Mute Add Command");
        if (_context is not InteractionContext interactionContext)
        {
            _logger.LogInformation("Not an interaction context");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got interaction context");
        var possibleGuild = interactionContext.Interaction.GuildID;
        if (!possibleGuild.IsDefined(out var guildId))
        {
            _logger.LogInformation("Not started from within a guild");
            return Result.FromSuccess();
        }

        if (await _moderationService.WriteMuteLine(guildId.Value, target.ID.Value, moderator.ID.Value, reason,
                                                   TimeSpan.Parse(muteDuration)))
            return (Result)await _feedbackService.SendContextualSuccessAsync("Successfully added new line");
        return (Result)await _feedbackService.SendContextualErrorAsync("Could not add new line");
    }

    [Command("warn")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Add a warning to the target user")]
    [Ephemeral]
    public async Task<IResult> AddUserWarning([Description("The moderator executing (aka you)")] IUser moderator,
                                              [Description("The user to warn")]                  IUser target,
                                              [Description("The reason for the warning")]
                                              string reason)
    {
        _logger.LogInformation("Received User Warn Add Command");
        if (_context is not InteractionContext interactionContext)
        {
            _logger.LogInformation("Not an interaction context");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got interaction context");
        var possibleGuild = interactionContext.Interaction.GuildID;
        if (!possibleGuild.IsDefined(out var guildId))
        {
            _logger.LogInformation("Not started from within a guild");
            return Result.FromSuccess();
        }

        if (await _moderationService.WriteWarningLine(guildId.Value, target.ID.Value, moderator.ID.Value, reason))
            return (Result)await _feedbackService.SendContextualSuccessAsync("Successfully added new line");
        return (Result)await _feedbackService.SendContextualErrorAsync("Could not add new line");
    }

    [Command("kick")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Add a kick to the target user")]
    [Ephemeral]
    public async Task<IResult> AddUserKick([Description("The moderator executing (aka you)")] IUser moderator,
                                           [Description("The user to kick")]                  IUser target,
                                           [Description("The reason for the kick")]
                                           string reason)
    {
        _logger.LogInformation("Received User Kick Add Command");
        if (_context is not InteractionContext interactionContext)
        {
            _logger.LogInformation("Not an interaction context");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got interaction context");
        var possibleGuild = interactionContext.Interaction.GuildID;
        if (!possibleGuild.IsDefined(out var guildId))
        {
            _logger.LogInformation("Not started from within a guild");
            return Result.FromSuccess();
        }

        if (await _moderationService.WriteKickLine(guildId.Value, target.ID.Value, moderator.ID.Value, reason))
            return (Result)await _feedbackService.SendContextualSuccessAsync("Successfully added new line");
        return (Result)await _feedbackService.SendContextualErrorAsync("Could not add new line");
    }

    [Command("ban")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Add a ban to the target user")]
    [Ephemeral]
    public async Task<IResult> AddUserBan([Description("The moderator executing (aka you)")] IUser  moderator,
                                          [Description("The user to ban")]                   IUser  target,
                                          [Description("The reason for the ban")]            string reason,
                                          [Description("The duration of the ban")]          string muteDuration)
    {
        _logger.LogInformation("Received User Warn Add Command");
        if (_context is not InteractionContext interactionContext)
        {
            _logger.LogInformation("Not an interaction context");
            return Result.FromSuccess();
        }

        _logger.LogInformation("Got interaction context");
        var possibleGuild = interactionContext.Interaction.GuildID;
        if (!possibleGuild.IsDefined(out var guildId))
        {
            _logger.LogInformation("Not started from within a guild");
            return Result.FromSuccess();
        }

        if (await _moderationService.WriteBanLine(guildId.Value, target.ID.Value, moderator.ID.Value, reason, TimeSpan.Parse(muteDuration)))
            return (Result)await _feedbackService.SendContextualSuccessAsync("Successfully added new line");
        return (Result)await _feedbackService.SendContextualErrorAsync("Could not add new line");
    }
}