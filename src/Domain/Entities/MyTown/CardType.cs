using System.ComponentModel.DataAnnotations;
using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;

namespace MyTown.Domain
    {
    //town table
    //townitemtype master data table
    //townid+ townitem table



    //fetch city related all entities at once & store on client side
    //display city title.subtitle,decription
    //priority & posible solution
    //events
    //business
    //doctor
    //school
    //college
    //real estate
    //buyorsale
    //openissue
    //report complaint
    //jobs vacancies & registrqation of job looking candidates in town
    //feedback or contact
    //no db entity


    //db entity
    public class CardType : BaseAuditableEntity, IMasterData
        {//this is only masterdata

        [Key]
        public int IdCardType { get; set; }
        public CardType()
            {
            Name = string.Empty;
            ShortName = string.Empty;
            }
        public CardType(string name)
            {
            Name = name;
            ShortName = name;
            }
        public CardType(string name, string shortName)
            {
            Name = name;
            ShortName = shortName;
            }

        public CardType(int id, string name)
            {
            IdCardType = id;
            Name = name;
            ShortName = name;
            }
        public CardType(int id, string name, string shortName)
            {
            IdCardType = id;
            Name = name;
            ShortName = shortName;
            }

        //[Key]
        //public int Id { get; set; }
        //this should not  be appeared to front end screen to users
        //    public byte ApplicationTypeId { get; set; } = 1;//1 for townitem,2 for holige products
        public string Name { get; set; }//1:Town,2doctor,business,event,advertisement
        public string ShortName { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; } = 1;//100
        public byte? PriorityOrder { get; set; } = 1;

        public byte RequiredApprovalCount { get; set; } = 1;

        //todo later will make it up in each card by user charging onrequest moving up or first kind of

        //not using this,instead using automapper
        //public void Update(string name, string shortName, string description, int price)
        //    {
        //    Name = name;
        //    ShortName = shortName;
        //    Description = description;
        //    Price = price;
        //    }
        /*
        public static TownCardType? Get(int id)
            {
            return StandardList.Find(x => x.Id == id);
            }
        public static TownCardType? GetFirst(string nameContains)
            {
            return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
        public static List<TownCardType>? GetList(string nameContains)
            {
            return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        public static readonly List<TownCardType> StandardList = new List<TownCardType>(){
            new TownCardType(0,"Town"),
            new TownCardType(1,"Priority Message"),//flash message
            new TownCardType(2,"Event"),
                 new TownCardType(3,"Premium Shops"),
            new TownCardType(4,"Doctor Clinic Hospital"),
            new TownCardType(5,"School College Tuition"),

            //business types
            new TownCardType(11,"Vehicle Garage Bike Car Scooter","Vehicle Garage"),
            new TownCardType(11,"Hotel Lodge Restaurant"),
            new TownCardType(11,"Textiles Tailors Designers"),
            new TownCardType(11,"Beauticians Saloons Hair Cut"),
            new TownCardType(11,"Electricals Home Appliances"),
            new TownCardType(11,"Choultry & Convention Hall"),
            new TownCardType(11,"Shops,Provision Stores,Super Markets"),//Jewellary,saw mills
            new TownCardType(11,"Gas Agency Petrol Bunks"),
            new TownCardType(11,"Bank,Govt Offices"),


             new TownCardType(7,"Real Estate"),
            new TownCardType(8,"Buy Or sale"),
            new TownCardType(9,"Open Issue"),
            new TownCardType(10,"Jobs Available"),
            new TownCardType(11,"Add Resume"),
            //user complaints
            };
        */
        }

    }
