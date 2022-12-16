using JonnyModerationHelper.Member.Abstractions;

namespace JonnyModerationHelper.Services.Abstractions;

public interface IMemberModerationService
{

    public IEnumerable<ILine> GetMemberInfo(ulong      guildId, LineQuerySelector selector);
    public Task               WriteLine(ulong guildId, IWriteableLine line);

}