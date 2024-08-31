using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;


namespace MyTown.Domain;

public class UserTrash : BaseAuditableEntity
    {
    //non auto generated key
    public required Guid UserId { get; set; }
    public required string UserDataAsJsonString { get; set; }
    }

