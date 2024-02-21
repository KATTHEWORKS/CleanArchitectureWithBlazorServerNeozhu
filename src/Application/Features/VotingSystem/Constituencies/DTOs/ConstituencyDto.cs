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
    public string? StateName {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Existing Mp Name")]
    public string? ExistingMpName {get;set;} 
    [Description("Alternate Mp Names")]
    public string? AlternateMpNames {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Read Count")]
    public int ReadCount {get;set;} 
    [Description("Write Count")]
    public int WriteCount {get;set;} 


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Constituency, ConstituencyDto>().ReverseMap();
        }
    }
}

