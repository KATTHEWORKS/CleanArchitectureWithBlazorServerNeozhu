using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PublicCommon.Common;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using PublicCommon;

namespace MyTown.Domain;

public class CardTag : BaseAuditableEntity
    {
    //here its actually one tag for one town,so had to be unique with townid
    //later we can plan for multiple taf for single town or card
    [Required]
    public int IdCard { get; set; } //bhadravathi,kadur,bidar

    [ForeignKey(nameof(IdCard))]
    public virtual Card? DraftCard { get; set; }


    //here its actually one tag for one town,so had to be unique with townid
    public required string Tag { get; set; }
    }