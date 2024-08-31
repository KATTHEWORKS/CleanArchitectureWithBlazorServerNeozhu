using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Domain.Entities;


public class VerifiedCard : CardBase, IEquatable<VerifiedCard>
{
    [Key]
    public override int Id { get; set; }

    public virtual ICollection<VerifiedCardRating>? VerifiedCardRatings { get; set; }
    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    //[DatabaseGenerated(DatabaseGeneratedOption.None)] // Disable identity column]//this is not working use fluent on ApplicationDbContext
    //[Key]
    //public int IdCard { get; set; }
    //no direct operation here by creator,instead all goes to card
    public VerifiedCard()
    {
        VerifiedCardDisplayDates = [];//new HashSet<TownVerifiedCardSelectedDate>();
        if (Guid.TryParse(CreatedBy, out var by))
            IdOwner = by;  //LastModifiedBy ?? CreatedBy;
    }
    public VerifiedCard(int typeId, string title) : this()
    {
        IdCardType = typeId;
        Title = title;
    }
    public void AddDate(VerifiedCardDisplayDate date)
    {
        // The HashSet will handle uniqueness and performance efficiently
        VerifiedCardDisplayDates.Add(date);
    }

    public virtual ICollection<VerifiedCardDisplayDate> VerifiedCardDisplayDates { get; set; }

    public bool IsAdminVerified { get; set; }
    public bool IsPeerVerified { get; set; }

    //This requires on every Verified/Reject needs to update these
    //public int VerifiedPeerCount { get; set; } //by equal or above grade people
    //public int RejectedPeerCount { get; set; } //by equal or above grade people


    //TownItemLikeDisLike
    public int LikeCount { get; set; }//by public anyone
    public int DisLikeCount { get; set; }//by public anyone

    public bool IsHotSearchable { get; set; }
    public ICollection<Town2VerifiedCard>? VerifiedCardTowns { get; set; } = new List<Town2VerifiedCard>();

    //[ForeignKey(nameof(IdCard))]//dont mention here
    public Card? DraftCard { get; set; }

    //this wont check CardData or CardDetails
    public bool EqualImages(VerifiedCard? other)//wont compare id
    {
        return EqualImages(this, other);
    }

    //this wont check CardData or CardDetails
    public bool Equals(VerifiedCard? other)//compares including id
    {//usage bool isEqual1 = person1.Equals(person2);
        if (other == null) return false; // Not the same type
        return Id == other.Id && Equals(this, other);
    }

    //this wont check CardData or CardDetails
    public static bool Equals(VerifiedCard? source, VerifiedCard? other)//compares including id
    {
        var baseCompare = CardBase.Equals(source, other);
        if (baseCompare)//then check remaining comparison
        {
            if (source == null && other == null) return true;
            else if (source == null || other == null) return false;
            return source.IsAdminVerified == other.IsAdminVerified && source.IsPeerVerified == other.IsPeerVerified
                && source.LikeCount == other.LikeCount && source.DisLikeCount == other.DisLikeCount;
        }
        else return false;
    }

    public static bool Equals(VerifiedCard? verified, Card? draft)//compares including id
    {
        return CardBase.Equals(verified, draft) && draft?.Id == verified?.Id;
    }
}


