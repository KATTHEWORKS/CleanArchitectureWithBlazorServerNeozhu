// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;

[Description("Votes")]
public class VoteDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("User Id")]
    public string? UserId {get;set;} 
    [Description("Constituency Id")]
    public int ConstituencyId {get;set;} 
    [Description("Constituency Id Delta")]
    public int? ConstituencyIdDelta {get;set;} 


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Vote, VoteDto>().ReverseMap();
        }
    }
}

