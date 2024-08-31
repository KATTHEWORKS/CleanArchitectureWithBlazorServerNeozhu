// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;

[Description("CardTypes")]
public class CardTypeDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Short Name")]
    public string? ShortName {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Price")]
    public int Price {get;set;} 
    [Description("Priority Order")]
    public byte? PriorityOrder {get;set;} 
    [Description("Required Approval Count")]
    public byte? RequiredApprovalCount {get;set;} 


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CardType, CardTypeDto>().ReverseMap();
        }
    }
}

