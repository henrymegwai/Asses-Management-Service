namespace CloudWorks.Data.Entities;
public sealed class Profile : BaseEntity
{
    public string Email { get; set; } = null!;
    public Guid? IdentityId  { get; set; }
}
