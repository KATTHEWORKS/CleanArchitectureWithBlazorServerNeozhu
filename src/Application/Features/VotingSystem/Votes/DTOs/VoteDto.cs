// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;

[Description("Votes")]
public class VoteDto
{//for showing in list of search page on grid
    [Description("Id")]
    public int Id { get; set; }

    [Description("User Id")]
    public string? UserId { get; set; }//not in UI,set from logged in

    [Description("Constituency Id")]
    public int ConstituencyId { get; set; } //not in UI,set from selected constituency

    //below 3 are in Add/EditCommand also
    [Description("Constituency Name")]
    public string? ConstituencyName { get; set; }//only for UI

    [Description("Existing MP Name")]
    public string? MpNameExisting { get; set; }//only for UI
    [Description("Existing MP Terms")]
    public string? ExistingMpTerms { get; set; }//only for UI


    // [Required] //overall rating
    public sbyte Rating { get; set; }//auto-generate based on kpi values

    [Description("Open Issues/Problems Needs to be addressed")]
    public string? OpenIssues { get; set; }

    public bool? WishToReElectMp { get; set; } = null;//yes/no/no answer

    public DateTime? Created { get; set; } = DateTime.Now;//for display only

    public DateTime? LastModified { get; set; }//for display only

    //[Description("Constituency Id Delta")]
    //public int? ConstituencyIdDelta {get;set;} //only for backend purpose

    //need to think is below are required in this model or not
    public List<KPIComment>? KPIComments { get; set; }//for the sake of summary page,this will appear without any person name,so stays anonymous
    //public bool IsCommentExists() => VoteKPIRatingComments.Any(x => !string.IsNullOrEmpty(x.Comment) && x.Comment.Length > 3);
    //since comments of others also visible to all in summary page but not votes usually
    public List<KPIRatingComment>? KPIRatingComments { get; set; }//mostly for self

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Vote, VoteDto>().ReverseMap();
        }
    }
}

