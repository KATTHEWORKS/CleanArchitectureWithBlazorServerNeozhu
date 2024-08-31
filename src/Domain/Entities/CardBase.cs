using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;
using PublicCommon.Common;


namespace CleanArchitecture.Blazor.Domain.Entities
{
    //town table
    //townItemType master data table
    //townId+ townItem table



    //fetch city related all entities at once & store on client side
    //display city title.subtitle,description
    //priority & possible solution
    //events,sports
    //business
    //doctor
    //school
    //college
    //real estate
    //buyOrSale
    //openIssue
    //report complaint
    //Films ,Drama & Actions
    //jobs vacancies & registration of job looking candidates in town
    //feedback or contact

    //CardBase NonDbEntity
    public class CardBase : BaseAuditableEntity, IEquatable<CardBase>
    {
        public CardData? CardData { get; set; }

        public CardDetail? CardDetail { get; set; }//optionsla so virtual
                                                   //public virtual int Id { get; set; }


        //[NotMapped]
        //public int? IdTownFirst { get; set; }

        public CardBase()
        {
            Title = string.Empty;//only to avoid warnings
                                 //this should never be called,1ly for the sake of EF cores
        }
        public CardBase(string title)
        {
            Title = title;
        }

        public CardBase(string title, string subtitle) : this(title)
        {
            SubTitle = subtitle;
        }

        //public CardBase(int id, string title, string subtitle) : this(title, subtitle)
        //    {
        //    //this should be removed later,as id is from db or from screen its 0/null only
        //    IdCard = id;
        //    }

        [Required]
        [Display(Name = "iCard Type-Category")]
        [Range(1, 50, ErrorMessage = "iCard Type-Category Must be Selected")]
        public int IdCardType { get; set; }//doctor,event,business

        [ForeignKey(nameof(IdCardType))]
        public virtual CardType? Type { get; set; }
        //ideally this has to be set on transfer ownership kind of but //todo
        public Guid IdOwner { get; set; }

        [Required]
        [Display(Name = "Town")]
        [Range(1, 10000, ErrorMessage = "Town Must be Selected As Base")]
        public int IdTown { get; set; }
        //bhadravathi,kadur,bidar
        //todo currently 1-1 mapping,lets change this to 1 to many town mapping,so avoids cloning of cards

        // [ForeignKey(nameof(IdTown))] dont mention here as complicated/inherited case,do on OnModelCreation dbcontext
        public virtual Town? Town { get; set; }


        public bool Active { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "{0} Must be {2} & {1} characters")]
        public string Title { get; set; }

        [MaxLength(60, ErrorMessage = "SubTitle must be less than 60 characters.")]
        public string? SubTitle { get; set; }//qualification,type of business,home/hotel/veg/nonveg

        [MaxLength(150, ErrorMessage = "Short Note must be less than 150 characters.")]
        public string? ShortNote { get; set; }

        [MaxLength(150, ErrorMessage = "Address must be less than 150 characters.")]
        public string? Address { get; set; }




        public string? Image1 { get; set; }

        public string? Image2 { get; set; }
        public void ResetImages()
        {
            Image1 = null;
            Image2 = null;
        }

        public static bool IsNullOrDefaultImages(CardBase? card) => card == null ||
     !ImageInfoBase64Url.IsUrl(card.Image1) && !ImageInfoBase64Url.IsUrl(card.Image2);


        public virtual bool EqualImages(CardBase? otherCard)//compares without id
        {//usage bool isEqual1 = person1.EqualImages(person2);
            if (otherCard == null) return false; // Not the same type

            //IdCardBrand == otherCard.IdCardBrand //here wont check for id
            return EqualImages(this, otherCard); // Compare properties
        }
        public static bool EqualImages(CardBase source, CardBase? otherDetails)//compares without id
        {
            if (otherDetails == null) return false; // Not the same type

            //IdCardBrand == otherCard.IdCardBrand //here wont check for id
            return
                StringExtensions.Equals(source.Image1, otherDetails.Image1) &&
                StringExtensions.Equals(source.Image2, otherDetails.Image2);
        }


        public bool Equals(CardBase? otherDetails)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
            if (otherDetails == null) return false; // Not the same type
            return Equals(this, otherDetails);
        }

        public static bool Equals(CardBase? source, CardBase? otherDetails)//compares including id
        {//usage bool isEqual1 = person1.Equals(person2);
            if (source == null && otherDetails == null) return true; // Not the same type
            if (source == null || otherDetails == null) return false;
            //id card compare on derived
            return source.IdCardType == otherDetails.IdCardType &&
                source.IdOwner == otherDetails.IdOwner && source.IdTown == otherDetails.IdTown &&
                source.Active == otherDetails.Active && //later
                StringExtensions.Equals(source.Title, otherDetails.Title) &&
                StringExtensions.Equals(source.SubTitle, otherDetails.SubTitle) &&
                StringExtensions.Equals(source.ShortNote, otherDetails.ShortNote) &&
                StringExtensions.Equals(source.Address, otherDetails.Address) &&
              EqualImages(source, otherDetails); // Compare properties
        }


        //2 types of images...
        //1 as BrandMainImages(1 or 2) ,fixed and always needs approval on change...
        //2 as MenuProducts(1 to 4) or flexible items o products...
        //todo remove this also
        //public string? MoreImages { get; set; }
        //storing <index>:<CARDID>.BrandMainImage-filename.extension,<index>:<CARDID>.BrandMainImage-filename.extension
        // above needs approval on change,but not belowone
        //public string? MenuProductImagesCsv { get; set; }//storing <index>:<CARDID>.MenuProductsImage-filename.extension,<index>:<CARDID>.MenuProductsImage-filename.extension,<index>:<CARDID>.MenuProductsImage-filename.extension...this dont need approval on change

        //"storageaccounc.om/1.jpg,storageaccounc.om/2.png,storageaccounc.om/3.jpeg,google.com/4.jpeg"

        /*
        public void Update(string name, string subTitle, string description, int price)
            {
            Name = name;
            SubTitle = subTitle;
            Description = description;
            }*/

    }


}
