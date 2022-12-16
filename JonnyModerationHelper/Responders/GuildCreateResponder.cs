using DatabaseManagement.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace JonnyModerationHelper.Responders;

public class GuildCreateResponder : IResponder<IGuildCreate>
{

    private readonly IDatabaseManager _databaseManager;

    public GuildCreateResponder(IDatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }
    
    public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = new CancellationToken())
    {
        await _databaseManager.EnsureSchemaUpToDate("guild" + gatewayEvent.ID.Value);
        return Result.FromSuccess();
    }
    
}