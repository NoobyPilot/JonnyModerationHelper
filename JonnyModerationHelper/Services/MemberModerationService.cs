using JonnyModerationHelper.Member.Abstractions;
using JonnyModerationHelper.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace JonnyModerationHelper.Services;

public class MemberModerationService : IMemberModerationService
{

    private readonly IModerationDatabaseService       _database;
    private readonly ILogger<MemberModerationService> _logger;

    public MemberModerationService(IModerationDatabaseService database, ILogger<MemberModerationService> logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public IEnumerable<ILine> GetMemberInfo(ulong guildId, LineQuerySelector selector)
    {
        _logger.LogInformation("Grabbing line IDs from database");
        var lines = _database.GetLines(guildId, selector).GetAwaiter().GetResult();
        _logger.LogInformation("Got information from database, now reading the lines");
        return lines.Select(line => _database.GetLine(guildId, line.Id).GetAwaiter().GetResult());
    }

    public Task WriteLine(ulong guildId, IWriteableLine line)
    {
        return _database.WriteLine(guildId, line);
    }
}