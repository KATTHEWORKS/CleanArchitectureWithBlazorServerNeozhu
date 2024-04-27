// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Domain.MyVote;
using PublicCommon.MyVote;

namespace Dto;

[Description("Votes")]
public class VoteDto
    {//for showing in list of search page on grid
    [Description("Id")]
    public int Id { get; set; }

    [Description("User Id")]
    public Guid? UserId { get; set; }//not in UI,set from logged in

    [Description("Constituency Id")]
    public int ConstituencyId { get; set; } //not in UI,set from selected constituency

    //below 3 are in Add/EditCommand also
    [Description("Constituency Name")]
    public string? ConstituencyName { get; set; }//only for UI

    [Description("State Name")]
    public string? State { get; set; }

    [Description("Current Member Name")]
    public string? CurrentMemberName { get; set; }//only for UI

    [Description("Current Member Party")]
    public string? CurrentMemberParty { get; set; }


    [Description("Current Member Terms")]
    public string? CurrentMemberTerms { get; set; }

    [Description("Constituency Rating By Over All")]
    public sbyte? ConstituencyRatingByOverAll { get; set; }//not for self

    // [Required] //overall rating
    public sbyte Rating { get; set; }//auto-generate based on kpi values

    //[Description("Open Issues/Problems Needs to be addressed")]
    //public string? OpenIssues { get; set; }//this covered under KPI itself,so separate is not necessary

    public bool? WorkDoneQuality { get; set; } = null;//yes/no/no answer

    public DateTime Created { get; set; } = DateTime.Now;//for display only

    public DateTime? LastModified { get; set; }//for display only

    //[Description("Constituency Id Delta")]
    //public int? ConstituencyIdDelta {get;set;} //only for backend purpose

    //need to think is below are required in this model or not
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
            Rating = VoteHelperExtensions.CalculateRating(value);
            }
        }

    //now this is not in use,bcz allCOnstituency results is not based on any userid...
    public bool? MySupportAsAViewer { get; set; }//this is only view perspective ,not only from voter,for clientside
    public int SupportCount { get; set; }//should not be set by user self
    public int OpposeCount { get; set; }//should not be set by user self
    public ICollection<VoteMessageSupportOppose>? VoteMessageSupportOpposes { get; set; }
    //this is this vote likes & comments

    [Description("My Rating")]
    public sbyte? RatingForUiPurposeInNegativeRangeOnlyForDisplay { get; set; }//auto-generate based on kpi values//make it readonly from user but 
    private class Mapping : Profile
        {
        public Mapping()
            {
            CreateMap<Vote, VoteDto>().ReverseMap();
            }
        }
    }

