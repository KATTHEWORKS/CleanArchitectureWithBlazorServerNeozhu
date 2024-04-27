
using Domain.MyVote;
using PublicCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MyTown
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
    public class TownBase : BaseAuditableEntityMultiUser
        {
        public TownBase()
            {
            //this should never be called,1ly for the sake of EF cores
            }
        public TownBase(ItemTypeCardMasterData type, string title)
            {
            Type = type;
            Title = title;
            }
        public TownBase(int townItemTypeId, string title) : this(ItemTypeCardMasterData.Get(townItemTypeId), title)
            {
            }
        public TownBase(int townItemTypeId, string title, string subtitle) : this(ItemTypeCardMasterData.Get(townItemTypeId), title)
            {
            SubTitle = subtitle;
            }

        public TownBase(ItemTypeCardMasterData type, string title, string subtitle) : this(type, title)
            {
            SubTitle = subtitle;
            }
        public TownBase(ItemTypeCardMasterData type, int id, string title, string subtitle) : this(type, title, subtitle)
            {
            //this should be removed later,as id is from db or from screen its 0/null only
            Id = id;
            }

        [Required]
        public int TypeId { get; set; }//doctor,event,business

        [ForeignKey(nameof(TypeId))]
        public ItemTypeCardMasterData Type { get; set; }


        public string Title { get; set; }
        public string? SubTitle { get; set; }//qualification,type of business,home/hotel/veg/nonveg
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }

        public string? MobileNumber { get; set; }
        public string? GoogleMapAddressUrl { get; set; }

        public bool IsActive { get; set; }
        public DateTime EndDateToShow { get; set; }//after this date content will be removed on screen
        public int PriotiryOrder { get; set; }

        public string? GoogleProfileUrl { get; set; }
        public string? FaceBookUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? InstagramUrl { get; set; }

        public string? TwitterUrl { get; set; }

        public string? OtherReferenceUrl { get; set; }


        }
    public class TownItem : TownBase
        {
        [Required]
        public int TownId { get; set; } //bhadravathi,kadur,bidar

        [ForeignKey(nameof(TownId))]
        public Town Town { get; set; }
        }

    public class Town : TownBase
        {
        public Town()
            {

            }
        [Key]
        public int TownId { get; set; }

        [NotMapped]
        public override int Id { get; set; }
        //public static readonly List<Town> SampleData = [Bhadravathi.Data];
        public string UrlName1 { get; set; }//bhadravathi.com
        public string UrlName2 { get; set; }//bdvt.in

        //public string? TownImageUrl1 { get; set; }
        //public string? TownImageUrl2 { get; set; }
        //public string? TownImageUrl3 { get; set; }
        //public string? TownImageUrl4 { get; set; }
        //public string? TownImageUrl5 { get; set; }
        //public string? TownYoutubeVideo { get; set; }

        // Navigation property for TownItems
        public ICollection<TownItem> TownItems { get; set; } = new List<TownItem>();
        }

    public class ItemTypeCardMasterData : BaseAuditableEntitySingleUser, IMasterData
        {//this is only masterdata
        public ItemTypeCardMasterData()
            {

            }
        public ItemTypeCardMasterData(int id, string name)
            {
            Id = id;
            Name = name;
            }
        public ItemTypeCardMasterData(int id, string name, string shortName) : this(id, name)
            {
            ShortName = shortName;
            }
        //[Key]
        //public int Id { get; set; }
        public int TypeId { get; set; } = 1;//townitem
        public string Name { get; set; }//doctor,business,event,advertisement
        public string ShortName { get; set; }
        public string BriefDescription { get; set; }
        public int Price { get; set; }//100
        public static ItemTypeCardMasterData? Get(int id)
            {
            return StandardList.Find(x => x.Id == id);
            }
        public static ItemTypeCardMasterData? GetFirst(string nameContains)
            {
            return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
        public static List<ItemTypeCardMasterData>? GetList(string nameContains)
            {
            return StandardList.Where(x => x.Name.Contains(nameContains, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        public static readonly List<ItemTypeCardMasterData> StandardList = [
            new ItemTypeCardMasterData(0,"Town"),
            new ItemTypeCardMasterData(1,"Priority Message"),//flash message
            new ItemTypeCardMasterData(2,"Event"),
                 new ItemTypeCardMasterData(3,"Premium Shops"),
            new ItemTypeCardMasterData(4,"Doctor Clinic Hospital"),
            new ItemTypeCardMasterData(5,"School College Tuition"),

            //business types
            new ItemTypeCardMasterData(11,"Vehicle Garage Bike Car Scooter","Vehicle Garage"),
            new ItemTypeCardMasterData(11,"Hotel Lodge Restaurant"),
            new ItemTypeCardMasterData(11,"Textiles Tailors Designers"),
            new ItemTypeCardMasterData(11,"Beauticians Saloons Hair Cut"),
            new ItemTypeCardMasterData(11,"Electricals Home Appliances"),
            new ItemTypeCardMasterData(11,"Choultry & Convention Hall"),
            new ItemTypeCardMasterData(11,"Shops,Provision Stores,Super Markets"),//Jewellary,saw mills
            new ItemTypeCardMasterData(11,"Gas Agency Petrol Bunks"),
            new ItemTypeCardMasterData(11,"Bank,Govt Offices"),


             new ItemTypeCardMasterData(7,"Real Estate"),
            new ItemTypeCardMasterData(8,"Buy Or sale"),
            new ItemTypeCardMasterData(9,"Open Issue"),
            new ItemTypeCardMasterData(10,"Jobs Available"),
            new ItemTypeCardMasterData(11,"Add Resume"),
            //user complaints
        ];
        }

    }
