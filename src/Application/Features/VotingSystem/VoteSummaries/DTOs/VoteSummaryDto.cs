// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;

[Description("VoteSummaries")]
public class VoteSummaryDto
{//to show on search page results
    public VoteSummaryDto()
    {
        VotesCountAgainstExistingMp = VotesCount - VotesCountForExistingMp ?? 0;
    }
    [Description("Id")]
    public int Id { get; set; }
    [Description("Constituency Id")]
    public int ConstituencyId { get; set; }
    [Description("Comments Count")]
    public int CommentsCount { get; set; }
    [Description("Votes Count")]
    public int VotesCount { get; set; } = 0;
    public sbyte? Rating { get; set; } = null;//this can be null when added user removed vote
    ////public float AggregateVote { get { return CalculateAggregateVote(); } }
    public int? VotesCountForExistingMp { get; set; } = 0;//from vote table  count(WishToReElectMp==true)
    public int? VotesCountAgainstExistingMp { get; set; } = 0;//from vote table  count(WishToReElectMp==false)

    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
    public List<KPIVote>? KPIVotes { get; set; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteSummary, VoteSummaryDto>().ReverseMap();
        }
    }
}

