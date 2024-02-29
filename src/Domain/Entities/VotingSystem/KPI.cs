using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Domain.Entities.VotingSystem;
public class KPI(int id, string nameShort, byte systemType = 1)
{
    public KPI(int id, string nameShort, string nameFull, string description, byte systemType = 1)
        : this(id, nameShort, systemType)
    {
        SubTitle = nameFull; Description = description;
        //other fields are passed to primary constructor
    }
    public KPI(int id, byte systemType = 1) : this(id, nameShort: id.ToString(), systemType)
    {
        var match = Standard.Find(x => x.Id == id);
        if (match != null)
        {
            SubTitle = match.SubTitle;
            Name = match.Name;
            Description = match.Description;
        }
    }
    public int Id { get; set; } = id;
    public string Name { get; set; } = nameShort;
    public string SubTitle { get; set; } = nameShort;//this is because if not passed in primary ctor only
    public string Description { get; set; } = nameShort;//this is because if not passed in primary ctor only

    public byte SystemType { get; set; } = systemType;//1 for MP had to later make some better method like enum & all 
    //this is to differentiate between others like visl strike,mp,mla all those


    public static readonly List<KPI> Standard = [
    new(1,"AdministrationTransparentPolitics","Administration & Transparent Politics","3 is best,-2 is very bad full of curruption",1),
        new(2,"HealthSectorQuality",1),
         new(3,"EducationSectorGovt",1),
    new(4,"IndustryDevelopmentExisitng" ,1),
    new(5,"IndustryBringingNew" ,1),
    new(6,"EmploymentCreationLocal",1),
    new(7,"RoadsAndInfrastructureDevelopment" ,1),

    new(8,"PublicTransport",1),

    new(9,"AgriculturalSupport",1),
    new(10,"EnvironmentalSafety",1),
    new(11,"RiotsAndSafetyControl",1),
    new(12,"FamilyAndCastePolitics",1)
];

    public static List<KPI> GetAllDefault(byte systemType = 1)
    {
        return Standard.Where(x => x.SystemType == systemType).ToList();
    }
    public static List<KPIRatingComment> GetAllDefaultAsVoteKPIRatingCommentList(byte systemType = 1)
    {
        return Standard.Where(x => x.SystemType == systemType).Select(x => new KPIRatingComment(x.Id)).ToList();
    }
    public static List<KPIRatingComment> MergeWithExistingVoteKPIRatingCommentList(List<KPIRatingComment> existing, byte systemType = 1)
    {
        var standardList = GetAllDefaultAsVoteKPIRatingCommentList(systemType);
        if (existing == null || existing.Count == 0) return standardList;
        foreach (var item in standardList)
        {
            if (!existing.Any(x => x.KPI_Id == item.KPI_Id))
            {
                existing.Add(new KPIRatingComment(item.KPI_Id));
            }
        }

        foreach (var item in existing.Where(x => x.KPI == null))
        {
            item.KPI = KPI.Get(item.KPI_Id);
        }
        //return existing.OrderBy(x => x.Rating).ThenBy(x => x.KPI).ToList();
        return [.. existing.OrderByDescending(x => x.Rating).ThenBy(x => x.KPI_Id)];
    }
    public static KPI? Get(int id, byte systemType = 1)
    {
        return Standard.Find(x => x.Id == id && x.SystemType == systemType);
    }

}
