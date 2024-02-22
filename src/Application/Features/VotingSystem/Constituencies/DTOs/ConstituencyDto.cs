// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities.VotingSystem;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;

[Description("Constituencies")]
public class ConstituencyDto
{
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


    [Description("Mp Names Earlier Others")]
    public string? MpNamesEarlierOthers { get; set; } = null;
    //name+party+terms


    [Description("Read Count")]
    public int ReadCount { get; set; } = 0;

    [Description("Write Count")]
    public int WriteCount { get; set; } = 0;


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Constituency, ConstituencyDto>().ReverseMap();
        }
    }
}

