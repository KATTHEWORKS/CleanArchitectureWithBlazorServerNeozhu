// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Domain;
using Domain.MyVote;
using Dto.MyVote;

namespace Dto;

[Description("Constituencies")]
public class ConstituencyDto
    {
    public ConstituencyDto()
        {
        // Summary ??= new();
        }
    [Description("Id")]
    public int Id { get; set; }

    [Description("State Name")]
    public string State { get; set; }

    [Description("Name")]
    public string Name { get; set; }

    [Description("Description")]
    public string? Description { get; set; }


    [Description("Current Member")]
    public string? CurrentMemberName { get; set; }

    [Description("Current Member Party")]
    public string? CurrentMemberParty { get; set; }

    [Description("Current Member Terms")]
    public string? CurrentMemberTerms { get; set; }


    [Description("Earlier Members Names")]
    public string? EarlierMembersNames { get; set; } = null;
    //name+party+terms
    [Description("Read Count")]
    public int ReadsCount { get; set; } = 0;//how many users looking for this,can increase by 1 each time on cache & write once after certain time

    //todo had to link summary
    public virtual VoteSummaryDto? Summary { get; set; } = new();

    public virtual List<VoteDto>? Votes { get; set; }//will be loaded later on screen

    ////below are from summary data table,as its keep changing.get from summary join result
    //public sbyte? Rating { get; set; } = null;//fetch from summary
    //public int? VoteCountForCurrentMember { get; set; } = 0;//fetch from summary
    //public int? VoteCountAgainstCurrentMember { get; set; } = 0;//fetch from summary
    //                                                         //public bool? ReElectSameCurrentMember { get; set; }



    //[Description("Vote Count")]//not in constituency db
    //public int VoteCount { get; set; } = 0;//not required,instead its useful in Summary

    public VoteDto? VoteOfLoggedInUser { get; set; }

    public bool? VoteUpdateSuccess { get; set; }
    public string? VoteUpdateResultMessage { get; set; }
    private class Mapping : Profile
        {
        public Mapping()
            {
            // CreateMap<VoteConstituency, ConstituencyDto>().ReverseMap();


            CreateMap<ConstituencyDto, VoteConstituency>()
    .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
    .ReverseMap();

            CreateMap<VoteConstituency, ConstituencyDto>()
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary ?? new VoteSummary()));
            //.ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary != null ? src.Summary : new VoteSummaryDto()));


            //need to map these from summary results if exists
            //     public sbyte? Rating { get; set; } = null;//fetch from summary
            //public int? VoteCounts { get; set; } = 0;//fetch from summary
            //public int? VoteCountsToReElectCurrentMember { get; set; } = 0;//fetch from summary
            //public int? VoteCountsToOpposeCurrentMember { get; set; } = 0;//fetch from summary
            }
        }
    }

