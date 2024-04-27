using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicCommon;

namespace Domain.MyVote;
//in case of voe deletion that should delete messageSuppor count also

//todo rename this as VoteSupportOppose
public class VoteMessageSupportOppose : BaseAuditableEntity
{
    public VoteMessageSupportOppose(int voteId, Guid supporterUserId, bool support)
    {
        VoteId = voteId;
        SupporterUserId = supporterUserId;
        Support = support;
        Created = DateTime.Now;
    }
    //[Key]
    ////[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    //public int Id { get; set; }//dont remove id,even its not directly needed.avoids tracking issues
    public int VoteId { get; set; }

    //[foreign key]
    [ForeignKey(nameof(VoteId))]
    public virtual Vote Vote { get; set; }

    [Required]
    public Guid SupporterUserId { get; set; }

    //[ForeignKey(nameof(UserId))]
    //public virtual ApplicationUser User { get; set; }
    public bool Support { get; set; } //True-Support
                                      //if nothing then remove the entry
}
