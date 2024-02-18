using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Identity;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Entities;
//https://sansad.in/


//dbtable2
public class V_Vote(int constituencyId, string userId) //1 user 1 row
{
    public V_Vote() : this(default, default)
    {
        Created = DateTime.Now;
    }

    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    public int Id { get; set; }
    //remove id and maintain userid only
    //or just keep id as counter,but always refer userid as key
    [Required]
    public string UserId { get; set; } = userId;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [Required]
    public int ConstituencyId { get; set; } = constituencyId;

    [ForeignKey(nameof(ConstituencyId))]
    public V_Constituency? Constituency { get; set; }

    [Required] //RatingEnum
    public sbyte Rating { get; set; }    //this has to be autogerated only then it makes great value for all,but curently not done anything for that

    [Required]
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public void UpdateModified()
    {
        Modified = DateTime.UtcNow;
    }
    //[Column(TypeName = "jsonb")] //this wont working mssql Adjust based on your database
    //public string? VotesJsonAsString { get; set; }//here comments are not stored

    //public string? VotesJsonAsStringDelta { get; set; }//this is for the sake of update difference tracking tyo summary table

    //[NotMapped]
    //public (int constituencyId, List<VoteKPIRatingComment> kpiRating) VoteKPIRatingCommentsDelta { get; set; }

    //public string? CommentsJsonAsString { get; set; }//lIST<KPI,COMMENT>

    //below propertiues will be used in Dtos,so here commenting

    //at viewmodel had to parse all bytes into string by (KPIEnum)value
   
    public List<VoteKPIComment>? VoteKPIComments { get; set; }//for the sake of summary page,this will appear without any person name,so stays anonymous

    //since comments of others also visible to all in summary page but not votes usually
    public List<VoteKPIRatingComment>? VoteKPIRatingComments { get; set; }//mostly for self
   
}

//in case of voe deletion that should delete commentSuppor count also
public class V_CommentSupportOppose
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    public int Vote_Id { get; set; }

    [ForeignKey(nameof(Vote_Id))]
    public V_Vote Vote { get; set; }


    [Required]
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }
    public bool Support { get; set; }
    //if nothing then remove the entry

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Modified { get; set; }
}
