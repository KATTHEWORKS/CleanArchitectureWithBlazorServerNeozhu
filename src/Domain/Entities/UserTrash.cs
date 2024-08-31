using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;


namespace CleanArchitecture.Blazor.Domain.Entities;

public class UserTrash : BaseAuditableEntity
{
    //non auto generated key
    public new Guid Id { get; set; }
    public required string UserDataAsJsonString { get; set; }
}

