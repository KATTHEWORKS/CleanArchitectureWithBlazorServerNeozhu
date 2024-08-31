using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;

namespace MyTown.Domain;


public class VerifiedCardDisplayDate : BaseAuditableEntity
    {
    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    
    public int IdCARD { get; set; }
    public DateOnly Date { get; set; }

    // Navigation property to the VerifiedCard
    [ForeignKey(nameof(IdCARD))]
    public virtual VerifiedCard? VerifiedCard { get; set; }
    }
