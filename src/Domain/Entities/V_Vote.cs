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
public class V_Vote //1 user 1 row
{
    public V_Vote()
    {
        Created = DateTime.Now;
    }

    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    public int Id { get; set; }
    //remove id and maintain userid only

    [Required]
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [Required]
    public int ConstituencyId { get; set; }

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
    public string? VotesJsonAsString { get; set; }//here comments are not stored

    public string? CommentsJsonAsString { get; set; }//lIST<KPI,COMMENT>

    //below propertiues will be used in Dtos,so here commenting

    //at viewmodel had to parse all bytes into string by (KPIEnum)value
    [NotMapped]
    public List<VoteKPIComment> VoteKPIComments//for the sake of summary page,this will appear without any person name,so stays anonymous
    {
        get => ((!string.IsNullOrEmpty(CommentsJsonAsString) && JsonExtensions.TryDeserialize<List<VoteKPIComment>>(CommentsJsonAsString, out var result1)) ? result1 : null);
        set
        {
            if (value != null)
                CommentsJsonAsString = JsonSerializer.Serialize(value.Where(c => !string.IsNullOrEmpty(c.Comment)).ToList(), JsonExtensions.IgnoreNullSerializationOptions);

        }
    }
    //since comments of others also visible to all in summary page but not votes usually
    [NotMapped]
    public List<VoteKPIRatingComment> VoteKPIRatingComments//mostly for self
    {//this comprises of vote & comments both 
        //so for summary page this should not be loaded
        //madhu continue here to address tweak


        //get => JsonSerializer.Deserialize<List<VoteKPIRatingComments>>(VotesJsonAsString);
        //get => ((!string.IsNullOrEmpty(VotesJsonAsString) && JsonExtensions.TryDeserialize<List<VoteKPIRatingComments>>(VotesJsonAsString, out var result)) ? result : null);
        get
        {
            if (string.IsNullOrEmpty(VotesJsonAsString)) return null;
            var temp1 = ((!string.IsNullOrEmpty(VotesJsonAsString) && JsonExtensions.TryDeserialize<List<VoteKPIRatingComment>>(VotesJsonAsString, out var result)) ? result : null);
            if (!string.IsNullOrEmpty(VotesJsonAsString) &&
                    (string.IsNullOrEmpty(CommentsJsonAsString) || VoteKPIComments.Count == 0))
                return temp1;
            else
                foreach (var item in VoteKPIComments)
                {
                    var toAddComments = temp1.Where(x => x.KPI == item.KPI).First();
                    if (toAddComments != null)
                        toAddComments.Comment = item.Comment;
                }
            return temp1;
        }
        set
        {
            if (value != null)
            {
                //VotesJsonAsString = JsonSerializer.Serialize(value);
                //for firsttime had to take all kpis so passing
                VotesJsonAsString = JsonSerializer.Serialize(value.Where(c => Id > 0 ? c.Rating != null : c.KPI > 0).ToList(), JsonExtensions.IgnoreNullSerializationOptions);

                var commentsList = value.Where(c => c.Rating != null && !string.IsNullOrEmpty(c.Comment)).Select(x => new VoteKPIComment() { KPI = x.KPI, Comment = x.Comment }).ToList();
                if (commentsList != null && commentsList.Count > 0)
                {
                    CommentsJsonAsString = JsonSerializer.Serialize(commentsList, JsonExtensions.IgnoreNullSerializationOptions);
                }
            }
        }
    }
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
