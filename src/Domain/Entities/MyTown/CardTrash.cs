using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;

namespace MyTown.Domain;

public class CardTrash : BaseAuditableEntity
    {
    //non auto generated key
    public required int IdCard { get; set; }
    public required string CardDataAsJsonString { get; set; }
    }

