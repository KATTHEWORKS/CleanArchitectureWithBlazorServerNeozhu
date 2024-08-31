using System.ComponentModel.DataAnnotations;

namespace MyTown.Domain
    {
    //later not yet included in db schema
    public class AdditionalDocuments
        {
        [Key]
        public virtual int Id { get; set; }
        }
    //images, files
    }
