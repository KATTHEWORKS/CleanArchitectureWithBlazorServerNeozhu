// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;

[Description("TypeOfProfileMasterDatas")]
public class TypeOfProfileMasterDataDto
{
    [Description("Id")]
    public int Id { get; set; }
        [Description("System Type Id")]
    public int SystemTypeId {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Short Name")]
    public string? ShortName {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Price")]
    public int Price {get;set;} 


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TypeOfProfileMasterData, TypeOfProfileMasterDataDto>().ReverseMap();
        }
    }
}

