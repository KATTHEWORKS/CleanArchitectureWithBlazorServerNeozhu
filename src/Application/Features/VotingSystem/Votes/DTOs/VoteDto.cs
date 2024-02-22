// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;

[Description("Votes")]
public class VoteDto
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("User Id")]
    public string? UserId { get; set; }//not in UI,set from logged in

    [Description("Constituency Id")]
    public int ConstituencyId { get; set; } //not in UI,set from selected constituency

    // [Required] //overall rating
    public sbyte Rating { get; set; }//auto-generate based on kpi values

    [Description("Open Issues/Problems Needs to be addressed")]
    public string? OpenIssues { get; set; }

    public bool? WishToReElectMp { get; set; } = null;//yes/no/no answer

    public DateTime? Created { get; set; } = DateTime.Now;//for display only

    public DateTime? LastModified { get; set; }//for display only

    //[Description("Constituency Id Delta")]
    //public int? ConstituencyIdDelta {get;set;} //only for backend purpose


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Vote, VoteDto>().ReverseMap();
        }
    }
}

