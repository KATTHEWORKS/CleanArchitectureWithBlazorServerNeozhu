using System.ComponentModel.DataAnnotations;
using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;

namespace CleanArchitecture.Domain.MyTown.Entities
    {

    //later not yet included in db schema
    public class Wallet : BaseAuditableEntity
        {

        [Key]
        public Guid UserId { get; set; } // Foreign key to ApplicationUser

        //[ForeignKey(nameof(UserId))]//its on differnt table so no link here
        //public ApplicationUser User { get; set; }

        public float Balance { get; set; }//decimal is constly

        // Other properties as needed
        }

    }
