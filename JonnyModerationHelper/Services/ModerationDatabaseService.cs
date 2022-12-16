using System.Text;
using DatabaseManagement.Abstractions;
using DatabaseManagement.Objects;
using JonnyModerationHelper.Member;
using JonnyModerationHelper.Member.Abstractions;
using JonnyModerationHelper.Member.Objects;
using JonnyModerationHelper.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace JonnyModerationHelper.Services;

public class ModerationDatabaseService : IModerationDatabaseService
{
    private const string PartialLinesQueryBlueprint =
        @"SELECT ID, CREATED_BY, CREATED_AT, LINE_TYPE, MODERATOR_ID, USER_ID
FROM guild{0}.MODERATION_LOG
WHERE TRUE";

    private const string FullLineBlueprint =
        @"SELECT ID, CREATED_BY, CREATED_AT, LINE_TYPE, MODERATOR_ID, USER_ID, VERSION, EDITED_AT, EDITED_BY, REASON, DURATION, CHANNEL
FROM guild{0}.MODERATION_LOG
WHERE ID = {1}";

    private const string NewLineBlueprint =
        @"INSERT INTO guild{0}.MODERATION_LOG(VERSION, CREATED_AT, CREATED_BY, LINE_TYPE, MODERATOR_ID, USER_ID, REASON) VALUES($1, $2, $3, $4, $5, $6, $7)";

    private const    string                             PartialLinesQueryOrder = @" ORDER BY CREATED_AT DESC";
    private readonly IDatabaseConnection                 _databaseConnection;
    private readonly ILogger<ModerationDatabaseService> _logger;

    public ModerationDatabaseService(IDatabaseConnection connection, ILogger<ModerationDatabaseService> logger)
    {
        _databaseConnection = connection;
        _logger = logger;
    }

    public async Task WriteLine(ulong guild, IWriteableLine line)
    {
        var query = string.Format(NewLineBlueprint, guild);
        var col = new List<NpgsqlParameter>()
        {
            new() { Value = line.Version },
            new() { Value = line.CreatedAt },
            new() { Value = (long)line.CreatedBy },
            new() { Value = (int)line.LineType },
            new() { Value = (long)line.ModeratorId },
            new() { Value = (long)line.UserId },
            new() { Value = line.Reason }
        };
        await _databaseConnection.ExecuteQuery(query, col);
    }

    public Task EditLine(ulong guild, ILine line)
    {
        throw new NotImplementedException();
    }

    public async Task<ILine> GetLine(ulong guild, long id)
    {
        var query = string.Format(FullLineBlueprint, guild, id);
        var reader = await _databaseConnection.ExecuteQuery(query);
        await reader.ReadAsync();
        return new Line(reader.GetInt64(0), (ulong)reader.GetInt64(1), reader.GetDateTime(2),
                        (InfoLineType)reader.GetInt32(3), (ulong)reader.GetInt64(4),
                        (ulong)reader.GetInt64(5), reader.GetInt64(6),
                        reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                        reader.IsDBNull(8) ? null : (ulong)reader.GetInt64(8), reader.GetString(9),
                        reader.IsDBNull(10) ? null : TimeSpan.FromSeconds(reader.GetInt32(10)),
                        reader.IsDBNull(11) ? null : reader.GetString(11));
    }

    public async Task<IEnumerable<IPartialLine>> GetLines(ulong             guild,
                                                          LineQuerySelector lineQuerySelector = new())
    {
        var queryBuilder = new StringBuilder(string.Format(PartialLinesQueryBlueprint, guild));
        if (lineQuerySelector.LineType != null)
        {
            queryBuilder.AppendFormat(" AND LINE_TYPE {0} {1}", (lineQuerySelector.ExactLineTypeMatch != null && !lineQuerySelector.ExactLineTypeMatch.Value?"=":">="), (int)lineQuerySelector.LineType.Value);
        }

        if (lineQuerySelector.ModeratorId != null)
        {
            queryBuilder.AppendFormat(" AND MODERATOR_ID = {0}", lineQuerySelector.ModeratorId.Value);
        }

        if (lineQuerySelector.UserId != null)
        {
            queryBuilder.AppendFormat(" AND USER_ID = {0}", lineQuerySelector.UserId.Value);
        }

        if (lineQuerySelector.CreatedSince != null)
        {
            var dt = lineQuerySelector.CreatedSince.Value;
            var dateTimeString =
                $"{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}:{dt.Millisecond}{dt.Microsecond}";
            queryBuilder.AppendFormat(" AND CREATED_AT >= {0}", dateTimeString);
        }

        queryBuilder.Append(PartialLinesQueryOrder);
        if (lineQuerySelector.Limit != null)
        {
            queryBuilder.AppendFormat(" LIMIT {0}", lineQuerySelector.Limit.Value);
        }

        if (lineQuerySelector.Offset != null)
        {
            queryBuilder.AppendFormat(" OFFSET {0}", lineQuerySelector.Offset.Value);
        }

        var query = queryBuilder.ToString();
        _logger.LogInformation("Executing partial line selector query");
        var reader = await _databaseConnection.ExecuteQuery(query);
        _logger.LogInformation($"Got reader with{(reader.HasRows?"":"out")} rows");
        if (!reader.HasRows)
        {
            _logger.LogInformation("Returning empty list");
            return new List<IPartialLine>();
        }
        var list = new List<IPartialLine>();
        while (await reader.ReadAsync())
        {
            _logger.LogInformation("Adding next line");
            list.Add(new PartialLine(reader.GetInt64(0), (ulong)reader.GetInt64(1), reader.GetDateTime(2),
                                     (InfoLineType)reader.GetInt32(3), (ulong)reader.GetInt64(4),
                                     (ulong)reader.GetInt64(5)));
        }
        _logger.LogInformation("Filled list");
        return list;
    }

    public Task<int> GetNumberOfLines(ulong guild, LineQuerySelector lineQuerySelector = new())
    {
        throw new NotImplementedException();
    }
    
}