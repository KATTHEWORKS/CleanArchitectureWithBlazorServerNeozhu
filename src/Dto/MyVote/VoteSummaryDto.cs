// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Domain.MyVote;
using PublicCommon.MyVote;

namespace Dto.MyVote;

[Description("VoteSummaries")]
public class VoteSummaryDto
{//to show on search page results
    public VoteSummaryDto()
    {
        WishToReElectMemberFalseCount = VotesCount - WishToReElectMemberTrueCount ?? 0;
    }
    [Description("Id")]
    public int Id { get; set; }
    [Description("Constituency Id")]
    public int ConstituencyId { get; set; }
    [Description("Messages Count")]
    public int MessagesCount { get; set; }
    [Description("Votes Count")]
    public int VotesCount { get; set; } = 0;
    public sbyte? Rating { get; set; } = null;//this can be null when added user removed vote
                                              ////public float AggregateVote { get { return CalculateAggregateVote(); } }

    [Description("Current Member Support Count")]
    public int? WishToReElectMemberTrueCount { get; set; } = 0;//from vote table  count(WishToReElectMember==true)

    [Description("Current Member Oppose Count")]
    public int? WishToReElectMemberFalseCount { get; set; } = 0;//from vote table  count(WishToReElectMember==false)

    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
    private List<KpiRatingCounts> KpiRatingCountsPrivate { get; set; }
    public List<KpiRatingCounts> KpiRatingCounts
    {
        get => KpiRatingCountsPrivate;
        set
        {
            KpiRatingCountsPrivate = value;
            if (value != null && value.Count > 0)
            {
                Rating = value.CalculateRating();
            }
        }
    }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteSummary, VoteSummaryDto>().ReverseMap();
        }
    }
}

