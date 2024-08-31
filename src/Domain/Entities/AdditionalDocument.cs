using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities
{
    //later not yet included in db schema
    public class AdditionalDocuments : BaseAuditableEntity
    {
        [Key]
        public override int Id { get; set; }
    }
    //images, files
}
