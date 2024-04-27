using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicCommon.MyVote;
public class KPI(int id, string name, byte systemType = 1)
{
    public KPI(int id, string name, string shortName, byte systemType = 1)
       : this(id, name, systemType)
    {
        ShortName = shortName;
        NameExpansionSubTitle = name;//since not passed expansion so assigning name itself
                                     //other fields are passed to primary constructor
    }
    public KPI(int id, string name, string shortName, string nameExpansionSubTitle, byte systemType = 1)
       : this(id, name, systemType)
    {
        ShortName = shortName;
        NameExpansionSubTitle = nameExpansionSubTitle;
    }
    public KPI(int id, string name, string shortName, string nameExpansionSubTitle, string description, byte systemType = 1)
        : this(id, name, systemType)
    {
        ShortName = shortName;
        NameExpansionSubTitle = nameExpansionSubTitle;
        Description = description;
    }

    public KPI(int id, byte systemType = 1) : this(id, name: id.ToString(), systemType)
    {
        var match = Standard.Find(x => x.Id == id);
        if (match != null)
        {
            NameExpansionSubTitle = match.NameExpansionSubTitle;
            Name = match.Name;
            Description = match.Description;
        }
    }
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ShortName { get; set; } = name;//for displaying on mobile
    public string NameExpansionSubTitle { get; set; } //this is because if not passed in primary ctor only
    public string Description { get; set; }//this is because if not passed in primary ctor only

    public byte SystemType { get; set; } = systemType;//1 for Member had to later make some better method like enum & all 
    //this is to differentiate between others like visl strike,mp,mla all those

    public const int OpenIssuesKpiId = 99;
    private static readonly List<KPI> Standard = [
    //  id,name,                                    shortname       description       
    new(1,"Administration & Transparent Politics","Administration",1),
    new(2,"Quality of Govt Hospital & Health sector","Health Sector",1),
    new(3,"Govt School & Colleges Development","Education",1),
    new(4,"Existing Industry Development","Exisitng Industry",1),
    new(5,"New Industry Bringing & Development","New Industry",1),
    new(6,"Employment Creation For local","Employment",1),
    new(7,"Roads & Infrastructure Development", "Roads & Infra",1),

    new(8,"Govt Public Transport & Facilities","Public Transport",1),

    new(9,"Agricultural Support","Agricultural",1),
    new(10,"Environment Safety","Environment",1),
    new(11,"Riots,Safety Control","Riots & Safety",1),
    new(12,"Family,Caste Politics","Family/Caste Politics",1),
new(OpenIssuesKpiId,"Any Other Issues")
];

    public static List<KPI> GetAllDefault(byte systemType = 1)
    {
        return Standard.Where(x => x.SystemType == systemType).ToList();
    }
    public static List<KPIRatingMessage> GetAllDefaultAsVoteKPIRatingMessageList(byte systemType = 1)
    {
        return Standard.Where(x => x.SystemType == systemType).Select(x => new KPIRatingMessage(x.Id)).ToList();
    }
    public static List<KPIRatingMessage> MergeWithCurrentVoteKPIRatingMessageList(List<KPIRatingMessage>? current, byte systemType = 1)
    {
        var standardList = GetAllDefaultAsVoteKPIRatingMessageList(systemType);
        if (current == null || current.Count == 0) return standardList;
        foreach (var item in standardList)
        {
            if (!current.Any(x => x.KPI_Id == item.KPI_Id))
            {
                current.Add(new KPIRatingMessage(item.KPI_Id));
            }
        }

        foreach (var item in current.Where(x => x.KPI == null))
        {
            item.KPI = Get(item.KPI_Id);
        }
        //return [.. current.OrderByDescending(x => x.Rating).ThenBy(x => x.KPI_Id)];
        //lets maintiain kpi always in same order on screen to be uniform
        return [.. current.OrderBy(x => x.KPI_Id)];
    }
    public static KPI? Get(int id, byte systemType = 1)
    {
        return Standard.Find(x => x.Id == id && x.SystemType == systemType);
    }

}
