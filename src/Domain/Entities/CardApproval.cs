using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Common.Entities;
namespace CleanArchitecture.Blazor.Domain.Entities;


public class CardApproval : BaseAuditableEntity
{
    //here admin entry will be as townidcard +admin id

    //Step1 user will choose approver
    //IdCard+IdCardOfApprover will be filled while selecting
    //step2 user will approve then will mark userif of particulr card owner,later can be tracked easily by which user and card owner all details

    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    [Required]
    //[DatabaseGenerated(DatabaseGeneratedOption.None)] // Disable identity column]//this is not working use fluent on ApplicationDbContext
    public override int Id { get; set; } //draftcardid for which approval taking

    [ForeignKey(nameof(Id))]
    public virtual Card? DraftCard { get; set; }

    [Required]//for admin this will be townidcard
    public int IdCardOfApprover { get; set; }

    [ForeignKey(nameof(IdCardOfApprover))]
    public virtual VerifiedCard? ApproverCard { get; set; }

    //when creator requested Verified is null then either he can approve or reject
    public bool? IsVerified { get; set; }

    public Guid? ApproverUserId { get; set; } = null;//can be admin or any other verified card owner with right

    public string? Message { get; set; }
}

