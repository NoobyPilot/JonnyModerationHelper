using Remora.Rest.Core;

namespace JonnyModerationHelper;

public class BaseEntity
{
    public int        Version   { get; private set; }
    public DateTime   CreatedAt { get; }
    public Snowflake  CreatedBy { get; }
    public DateTime?  EditedAt  { get; private set; }
    public Snowflake? EditedBy  { get; private set; }

    public BaseEntity(DateTime createdAt, Snowflake createdBy, DateTime? editedAt = null, Snowflake? editedBy = null,
                      int      version = 0)
    {
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        EditedAt = editedAt;
        EditedBy = editedBy;
        Version = version;
    }

    public BaseEntity(Snowflake createdBy) : this(DateTime.Now, createdBy)
    {
    }

    protected void MarkEdited(Snowflake editedBy)
    {
        EditedBy = editedBy;
        EditedAt = DateTime.Now;
        Version++;
    }
    
}