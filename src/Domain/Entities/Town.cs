using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;


public class Town : BaseAuditableEntity
{
    public Town()
    {
        //this should never be called,1ly for the sake of EF cores
    }
    public Town(TypeOfProfileMasterData type, string title)
    {
        TypeOfProfile = type;
        Name = title;
    }
    public Town(int townProfileTypeId, string title) : this(TypeOfProfileMasterData.Get(townProfileTypeId), title)
    {
    }
    public Town(int townProfileTypeId, string title, string subtitle) : this(TypeOfProfileMasterData.Get(townProfileTypeId), title)
    {
        SubTitle = subtitle;
    }

    public Town(TypeOfProfileMasterData type, string title, string subtitle) : this(type, title)
    {
        SubTitle = subtitle;
    }
    public Town(TypeOfProfileMasterData type, int id, string title, string subtitle) : this(type, title, subtitle)
    {
        //this should be removed later,as id is from db or from screen its 0/null only
        Id = id;
    }
    public string District { get; set; }//later move to other table called districts & refer here only id 
    public string State { get; set; }//later move to other table called states & refer here only id 

    public string UrlName1 { get; set; }//bhadravathi.com
    public string UrlName2 { get; set; }//bdvt.in


    public virtual ICollection<TownProfile> TownProfiles { get; set; } = new List<TownProfile>();
    [Required]
    public int TypeOfProfileId { get; set; }// 1 for town,doctor,event,business

    [ForeignKey(nameof(TypeOfProfileId))]
    public TypeOfProfileMasterData TypeOfProfile { get; set; } //= 1;

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


}


