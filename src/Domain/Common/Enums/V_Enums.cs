using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Identity;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Common.Enums;
//https://sansad.in/

//stop using enum & instead of this use class V_KPI
//public enum KPIEnum//this can be moved to db and read once & store in cache or static but later //TODO
//{
//    [Display(Name = "Administration & TransparentPolitics", Description = "3 is best,-2 is very bad full of curruption")]
//    AdministrationTransparentPolitics = 1,//    AdministrationTransparentPolitics,Corruption 

//    HealthSectorQuality = 2,
//    EducationSectorGovt = 3,//EducationSectorPrivate,
//    IndustryDevelopmentExisitng = 4,
//    IndustryBringingNew = 5,
//    EmploymentCreationLocal = 6,
//    RoadsAndInfrastructureDevelopment = 7,

//    PublicTransport = 8,

//    AgriculturalSupport = 9,
//    EnvironmentalSafety = 10,
//    RiotsAndSafetyControl = 11,
//    FamilyAndCastePolitics = 12
//}

public enum RatingEnum
{
    VeryBad = -2,
    Bad = -1,
    OkOk = 0,
    GoodPersonButUnableToDoDueToOtherCircumstances = 1,
    GoodWork = 2,
    GreatWork = 3
}

