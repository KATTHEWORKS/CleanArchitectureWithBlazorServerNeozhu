// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;

[Description("VoteSummaries")]
public class VoteSummaryDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("Constituency Id")]
    public int ConstituencyId {get;set;} 
    [Description("Comments Count")]
    public int CommentsCount {get;set;} 
    [Description("Votes Count")]
    public int VotesCount {get;set;} 


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteSummary, VoteSummaryDto>().ReverseMap();
        }
    }
}

