using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PublicCommon;
using PublicCommon.MyVote;

namespace Domain.MyVote;
//https://sansad.in/


//dbtable2
public class Vote(int constituencyId, Guid userId) : BaseAuditableEntitySingleUser //1 user 1 row
{
    public Vote() : this(default, default)
    {
        Created = DateTime.Now;
    }
    //[Key]
    ////[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    //public int Id { get; set; }//dont remove id,even its not directly needed.avoids tracking issues

    [Required]
    public Guid UserId { get; set; } = userId;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [Required]
    public int ConstituencyId { get; set; } = constituencyId;

    [ForeignKey(nameof(ConstituencyId))]
    public VoteConstituency? Constituency { get; set; }

    [Required] //overall rating
    public sbyte Rating { get; set; }    //this has to be autogerated only then it makes great value for all,but curently not done anything for that

    //public string? OpenIssues { get; set; }
    public bool? WorkDoneQuality { get; set; } = null;//yes/no/no answer

    public void UpdateTimeStamp()
    {
        Updated = DateTime.Now;
    }
    //[Column(TypeName = "jsonb")] //this wont working mssql Adjust based on your database
    //public string? VotesJsonAsString { get; set; }//here messages are not stored

    //public string? VotesJsonAsStringDelta { get; set; }//this is for the sake of update difference tracking tyo summary table

    //[NotMapped]
    public List<KPIRatingMessage>? KPIRatingMessagesDelta { get; set; }
    public int? ConstituencyIdDelta { get; set; }

    //public string? MessagesJsonAsString { get; set; }//lIST<KPI,Message>

    //below propertiues will be used in Dtos,so here messageing

    //at viewmodel had to parse all bytes into string by (KPIEnum)value

    public List<KPIMessage>? KPIMessages { get; set; }//for the sake of summary page,this will appear without any person name,so stays anonymous
    //public bool IsMessageExists() => VoteKPIRatingMessages.Any(x => !string.IsNullOrEmpty(x.Message) && x.Message.Length > 3);
    //since messages of others also visible to all in summary page but not votes usually
    //public List<KPIRatingMessage>? KPIRatingMessages { get; set; }//mostly for self
    private List<KPIRatingMessage>? _kpiRatingMessages;
    public List<KPIRatingMessage>? KPIRatingMessages
    {
        get => _kpiRatingMessages;
        set
        {
            _kpiRatingMessages = value;
            Rating = value.CalculateRating();
        }
    }

    public int SupportCount { get; set; }//should not be set by user self
    public int OpposeCount { get; set; }//should not be set by user self

    public void ResetSupportOpposeCounts()
    {
        SupportCount = default;
        OpposeCount = default;
    }

    //this is this vote likes & comments
    public virtual ICollection<VoteMessageSupportOppose>? VoteMessageSupportOpposes { get; set; }
    //this creates error of 
    //Introducing FOREIGN KEY constraint 'FK_VoteMessageSupportOpposes_Votes_VoteId' on table 'VoteMessageSupportOpposes' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
}


