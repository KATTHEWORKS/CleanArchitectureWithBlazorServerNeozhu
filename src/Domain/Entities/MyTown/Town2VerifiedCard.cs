using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;


namespace MyTown.Domain
    {

    public class Town2VerifiedCard : BaseAuditableEntity
        {
        public Town2VerifiedCard()
            {
            }
        public Town2VerifiedCard(int idTown, int idCard)
            {
            IdTown = idTown;
            IdCARD = idCard;
            }
        public int IdTown { get; set; }
        public int IdCARD { get; set; }

        //[ForeignKey(nameof(IdCard))]
        public virtual VerifiedCard? VerifiedCard { get; set; }

        //[ForeignKey(nameof(IdTown))]
        public virtual Town? Town { get; set; }
        }
    }
