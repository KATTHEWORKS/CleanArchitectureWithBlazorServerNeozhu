﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Identity;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Entities.VotingSystem;
//https://sansad.in/


//dbtable2
public class Vote(int constituencyId, string userId) : BaseAuditableEntity //1 user 1 row
{
    public Vote() : this(default, default)
    {
        Created = DateTime.Now;
    }

    //[Key]
    ////[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    //public int Id { get; set; }//dont remove id,even its not directly needed.avoids tracking issues

    [Required]
    public string UserId { get; set; } = userId;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [Required]
    public int ConstituencyId { get; set; } = constituencyId;

    [ForeignKey(nameof(ConstituencyId))]
    public Constituency? Constituency { get; set; }

    [Required] //overall rating
    public sbyte Rating { get; set; }    //this has to be autogerated only then it makes great value for all,but curently not done anything for that

    public string? OpenIssues { get; set; }
    public bool? WishToReElectMp { get; set; } = null;//yes/no/no answer

    public void UpdateModified()
    {
        LastModified = DateTime.UtcNow;
    }
    //[Column(TypeName = "jsonb")] //this wont working mssql Adjust based on your database
    //public string? VotesJsonAsString { get; set; }//here comments are not stored

    //public string? VotesJsonAsStringDelta { get; set; }//this is for the sake of update difference tracking tyo summary table

    //[NotMapped]
    public List<KPIRatingComment>? KPIRatingCommentsDelta { get; set; }
    public int? ConstituencyIdDelta { get; set; }

    //public string? CommentsJsonAsString { get; set; }//lIST<KPI,COMMENT>

    //below propertiues will be used in Dtos,so here commenting

    //at viewmodel had to parse all bytes into string by (KPIEnum)value

    public List<KPIComment>? KPIComments { get; set; }//for the sake of summary page,this will appear without any person name,so stays anonymous
    //public bool IsCommentExists() => VoteKPIRatingComments.Any(x => !string.IsNullOrEmpty(x.Comment) && x.Comment.Length > 3);
    //since comments of others also visible to all in summary page but not votes usually
    public List<KPIRatingComment>? KPIRatingComments { get; set; }//mostly for self

}

//in case of voe deletion that should delete commentSuppor count also
public class CommentSupportOppose
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    public int Vote_Id { get; set; }

    [ForeignKey(nameof(Vote_Id))]
    public Vote Vote { get; set; }


    [Required]
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }
    public bool Support { get; set; }
    //if nothing then remove the entry

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? LastModified { get; set; }
}
