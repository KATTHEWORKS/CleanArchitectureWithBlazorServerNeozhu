using CleanArchitecture.Blazor.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Domain.Entities
{

    public class TownProfile : BaseAuditableEntity
    {
        public TownProfile()
        {
            //this should never be called,1ly for the sake of EF cores
        }
        public TownProfile(TypeOfProfileMasterData type, string title)
        {
            TypeOfProfile = type;
            Name = title;
        }
        public TownProfile(int townProfileTypeId, string title) : this(TypeOfProfileMasterData.Get(townProfileTypeId), title)
        {
        }
        public TownProfile(int townProfileTypeId, string title, string subtitle) : this(TypeOfProfileMasterData.Get(townProfileTypeId), title)
        {
            SubTitle = subtitle;
        }

        public TownProfile(TypeOfProfileMasterData type, string title, string subtitle) : this(type, title)
        {
            SubTitle = subtitle;
        }
        public TownProfile(TypeOfProfileMasterData type, int id, string title, string subtitle) : this(type, title, subtitle)
        {
            //this should be removed later,as id is from db or from screen its 0/null only
            Id = id;
        }

        [Required]
        public int TypeOfProfileId { get; set; }//doctor,event,business

        [ForeignKey(nameof(TypeOfProfileId))]
        public TypeOfProfileMasterData TypeOfProfile { get; set; }

       
        [Required]
        public int TownId { get; set; } //bhadravathi,kadur,bidar

        [ForeignKey(nameof(TownId))]
        public Town Town { get; set; }


        public bool Active { get; set; }
        public string Name { get; set; }
        public string? SubTitle { get; set; }//qualification,type of business,home/hotel/veg/nonveg
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }

        public string? MobileNumber { get; set; }
        public string? GoogleMapAddressUrl { get; set; }


        public DateTime EndDateToShow { get; set; }//after this date content will be removed on screen
        public int? PriotiryOrder { get; set; }

        public string? GoogleProfileUrl { get; set; }
        public string? FaceBookUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? InstagramUrl { get; set; }

        public string? TwitterUrl { get; set; }

        public string? OtherReferenceUrl { get; set; }


        //separatetable "TownProfileApproval"
        public int ApprovedCount { get; set; } //by equal or above grade people
        public int RejectedCount { get; set; } //by equal or above grade people

        //TownItemLikeDisLike
        public int LikeCount { get; set; }//by public anyone
        public int DisLikeCount { get; set; }//by public anyone
    }

}
