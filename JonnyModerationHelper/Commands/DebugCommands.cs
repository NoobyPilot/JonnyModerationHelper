using DatabaseManagement.Abstractions;
using DatabaseManagement.Objects;
using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace JonnyModerationHelper.Commands;

[Group("debug")]
public class DebugCommands : CommandGroup
{

    private readonly FeedbackService        _feedbackService;
    private readonly IDatabaseConnection     _databaseConnection;
    private readonly ICommandContext        _commandContext;
    private readonly ILogger<DebugCommands> _logger;

    public DebugCommands(FeedbackService feedbackService, IDatabaseConnection databaseConnection, ICommandContext commandContext, ILogger<DebugCommands> logger)
    {
        _feedbackService = feedbackService;
        _databaseConnection = databaseConnection;
        _commandContext = commandContext;
        _logger = logger;
    }
    
    [Command("sql-query")]
    [CommandType(ApplicationCommandType.ChatInput)]
    public async Task<IResult> ExecuteSqlQuery(string sql)
    {
        _logger.LogInformation("AAAAAAAAAAAAAA");
        var reader = await _databaseConnection.ExecuteQuery(sql);
        var res = await _feedbackService.SendContextualAsync($"Reader received with \"{reader.Rows}\" rows");
        if (res.IsSuccess)
        {
            _logger.LogInformation("Successfully delivered debug query");
        }
        else
        {
            _logger.LogWarning("Error while delivering debug message");
        }
        return (Result)res;
    }
    
}