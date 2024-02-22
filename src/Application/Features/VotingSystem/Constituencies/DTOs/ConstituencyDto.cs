// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Domain.Entities.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;

[Description("Constituencies")]
public class ConstituencyDto
{
    public ConstituencyDto()
    {
        VoteCountAgainstExistingMp = VoteCount - VoteCountForExistingMp ?? 0;
    }
    [Description("Id")]
    public int Id { get; set; }

    [Description("State Name")]
    public string State { get; set; }

    [Description("Name")]
    public string Name { get; set; }

    [Description("Description")]
    public string? Description { get; set; }


    [Description("Mp Name Existing")]
    public string? MpNameExisting { get; set; }

    [Description("Existing Mp Party")]
    public string? ExistingMpParty { get; set; }

    [Description("Existing Mp Terms")]
    public string? ExistingMpTerms { get; set; }


    [Description("Other Past Mps")]
    public string? OtherPastMps { get; set; } = null;
    //name+party+terms
    [Description("Read Count")]
    public int ReadCount { get; set; } = 0;//how many users looking for this,can increase by 1 each time on cache & write once after certain time

    //todo had to link summary
    public virtual VoteSummaryDto? Summary { get; set; }//; = new();


    //below are from summary data table,as its keep changing.get from summary join result
    public sbyte? Rating { get; set; } = null;//fetch from summary
    public int? VoteCountForExistingMp { get; set; } = 0;//fetch from summary
    public int? VoteCountAgainstExistingMp { get; set; } = 0;//fetch from summary
                                                             //public bool? ReElectSameExistingMp { get; set; }



    [Description("Write Count")]//not in constituency db
    public int VoteCount { get; set; } = 0;//not required,instead its useful in Summary

  
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Constituency, ConstituencyDto>().ReverseMap();
            //need to map these from summary results if exists
            //     public sbyte? Rating { get; set; } = null;//fetch from summary
            //public int? VoteCounts { get; set; } = 0;//fetch from summary
            //public int? VoteCountsToReElectExistingMp { get; set; } = 0;//fetch from summary
            //public int? VoteCountsToOpposeExistingMp { get; set; } = 0;//fetch from summary
        }
    }
}

