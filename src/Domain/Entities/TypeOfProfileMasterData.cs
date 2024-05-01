using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;
public class TypeOfProfileMasterData : BaseAuditableEntity//, IMasterData
{//this is only masterdata
    public TypeOfProfileMasterData()
    {

    }
    public TypeOfProfileMasterData(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public TypeOfProfileMasterData(int id, string name, string shortName) : this(id, name)
    {
        ShortName = shortName;
    }
    //[Key]
    //public int Id { get; set; }
    public int SystemTypeId { get; set; } = 1;//1 for townitem,2 for holige products
    public string Name { get; set; }//1:Town,2doctor,business,event,advertisement
    public string ShortName { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }//100
    public static TypeOfProfileMasterData? Get(int id)
    {
        return StandardList.Find(x => x.Id == id);
    }
    public static TypeOfProfileMasterData? GetFirst(string nameContains)
    {
        return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
    }
    public static List<TypeOfProfileMasterData>? GetList(string nameContains)
    {
        return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }
    public static readonly List<TypeOfProfileMasterData> StandardList = [
        new TypeOfProfileMasterData(0,"Town"),
            new TypeOfProfileMasterData(1,"Priority Message"),//flash message
            new TypeOfProfileMasterData(2,"Event"),
                 new TypeOfProfileMasterData(3,"Premium Shops"),
            new TypeOfProfileMasterData(4,"Doctor Clinic Hospital"),
            new TypeOfProfileMasterData(5,"School College Tuition"),

            //business types
            new TypeOfProfileMasterData(11,"Vehicle Garage Bike Car Scooter","Vehicle Garage"),
            new TypeOfProfileMasterData(11,"Hotel Lodge Restaurant"),
            new TypeOfProfileMasterData(11,"Textiles Tailors Designers"),
            new TypeOfProfileMasterData(11,"Beauticians Saloons Hair Cut"),
            new TypeOfProfileMasterData(11,"Electricals Home Appliances"),
            new TypeOfProfileMasterData(11,"Choultry & Convention Hall"),
            new TypeOfProfileMasterData(11,"Shops,Provision Stores,Super Markets"),//Jewellary,saw mills
            new TypeOfProfileMasterData(11,"Gas Agency Petrol Bunks"),
            new TypeOfProfileMasterData(11,"Bank,Govt Offices"),


             new TypeOfProfileMasterData(7,"Real Estate"),
            new TypeOfProfileMasterData(8,"Buy Or sale"),
            new TypeOfProfileMasterData(9,"Open Issue"),
            new TypeOfProfileMasterData(10,"Jobs Available"),
            new TypeOfProfileMasterData(11,"Add Resume"),
            //user complaints
        ];
}
