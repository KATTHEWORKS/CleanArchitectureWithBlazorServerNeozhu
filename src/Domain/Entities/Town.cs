using System.ComponentModel.DataAnnotations;
using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Entities;

//db entity
public class Town : BaseAuditableEntity
{
    [Key]
    public override int Id { get; set; }
    public Town()
    {

    }
    public string Name { get; set; }//had to remove this or alter duplication of card
    public string? Name2 { get; set; }//had to remove this a duplicate of card
    public string? Description { get; set; }//had to remove this a duplicate of card

    public int? IdTownCard { get; set; }//only id,no relationship...so had to fetch separately & attach if needed

    public virtual Card? TownDraftCard { get; set; }
    public virtual VerifiedCard? TownVerifiedCard { get; set; }

    public List<Town2VerifiedCard> TownVerifiedCards { get; set; } = new List<Town2VerifiedCard>();



    //public iCardDraft? TownCardDraft { get; set; }
    public string? District { get; set; } //= "Shimoga";//later move to other table called districts & refer here only id 
    public string? State { get; set; } //= "Karnataka";//later move to other table called states & refer here only id 
    public string? UrlName1 { get; set; }//bhadravathi.com
    public string? UrlName2 { get; set; }//bdvt.in

    public DateTime? LastCardUpdateTime { get; set; } = DateTimeExtension.CurrentTime;
}

