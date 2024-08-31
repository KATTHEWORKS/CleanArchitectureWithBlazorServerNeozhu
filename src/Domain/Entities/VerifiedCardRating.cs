using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;


namespace CleanArchitecture.Blazor.Domain.Entities;

public class VerifiedCardRating : BaseAuditableEntity
{
    //IdCard+UserId
    [Required]
    public int IdCARD { get; set; } //bhadravathi,kadur,bidar

    [ForeignKey(nameof(IdCARD))]
    public virtual VerifiedCard? CardVerified { get; set; }

    [Display(Name = "ByUserId")]
    public Guid UserId { get; set; }

    public bool Liked { get; set; }
    //either like or rating anyone might be enough //later
    public int Rating { get; set; }
    public string? Message { get; set; }
}



