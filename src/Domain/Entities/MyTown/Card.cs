using System.ComponentModel.DataAnnotations;

namespace MyTown.Domain;
//each called iCard , internet card of any user or business entity
//dbentity, once verified then will be moving the verified entity to CardVerified table
public class Card : CardBase, IEquatable<Card>
{
    [Key]
    public override int Id { get; set; }

    public virtual ICollection<CardApproval>? CardApprovals { get; set; }

    public bool? IsSameAsVerified { get; set; }
    //once verified make it as true. then if any change on Draft table then change it to false

    // public virtual int Id { get; set; }//dont use the default id, to avoid confusion
    //[Key]
    //public int IdCard { get; set; }
    public Card()
    {
        if (Guid.TryParse(CreatedBy, out Guid by))
            IdOwner = by;  //LastModifiedBy ?? CreatedBy;
                           //this is must for EF cores migration & all
    }
    public Card(int typeId, string title) : this()
    {
        IdCardType = typeId;
        Title = title;
    }
    public Card(CardType type, string title) : this()
    {
        Type = type;
        Title = title;
    }
    public Card(CardType type, string title, string subtitle) : this(type, title)
    {
        SubTitle = subtitle;
    }
    public Card(CardType type, int id, string title, string subtitle) : this(type, title, subtitle)
    {
        //this should be removed later,as id is from db or from screen its 0/null only
        Id = id;
    }
    public bool? IsVerified { get; set; }//either admin or peerverified

    //[ForeignKey(nameof(VerifiedCardId))]//dont mention this
    public VerifiedCard? VerifiedCard { get; set; }

    public bool EqualImages(Card? other)//compares including id
    {
        return CardBase.EqualImages(this, other);
    }

    //this wont check CardData or CardDetails
    public bool Equals(Card? other)//compares including id
    {//usage bool isEqual1 = person1.Equals(person2);
        if (other == null) return false; // Not the same type
        return Id == other.Id && Equals(this, other);
    }

    //this wont check CardData or CardDetails
    public static bool Equals(Card? source, Card? other)//compares including id
    {//usage bool isEqual1 = person1.Equals(person2);

        var baseCompare = CardBase.Equals(source, other);

        if (baseCompare)//then check remaining comparison
        {
            if (source == null || other == null) return true;
            return source.IsVerified == other.IsVerified &&
                source.IsSameAsVerified == other.IsSameAsVerified;
        }
        else return false;
    }

    //this wont check CardData or CardDetails
    public static bool Equals(Card? draft, VerifiedCard? verified)//compares including id
    {
        return CardBase.Equals(verified, draft) && draft?.Id == verified?.Id;
    }
}
