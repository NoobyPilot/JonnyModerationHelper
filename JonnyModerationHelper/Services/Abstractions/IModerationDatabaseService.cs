using JonnyModerationHelper.Member.Abstractions;

namespace JonnyModerationHelper.Services.Abstractions;

public interface IModerationDatabaseService
{
    public Task                            WriteLine(ulong        guild, IWriteableLine             line);
    public Task                            EditLine(ulong         guild, ILine             line);
    public Task<ILine>                     GetLine(ulong          guild, long              id);
    public Task<IEnumerable<IPartialLine>> GetLines(ulong         guild, LineQuerySelector lineQuerySelector = new());
    public Task<int>                       GetNumberOfLines(ulong guild, LineQuerySelector lineQuerySelector = new());
}