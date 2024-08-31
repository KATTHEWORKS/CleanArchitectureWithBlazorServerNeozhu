using System.ComponentModel.DataAnnotations.Schema;

namespace PublicCommon.Common
    {
    public abstract class AuditableBaseEntity : BaseEntity
        {

        public virtual Guid UserId { get; set; }//for single user modifying entities

        public virtual Guid CreatedBy { get; set; }
        public virtual DateTime Created { get; set; } = DateTimeExtension.CurrentTime;

        public virtual DateTime? LastModified { get; set; }
        public virtual Guid? LastModifiedBy { get; set; }


        public void UpdateTimeStamp()
            {
            LastModified = DateTimeExtension.CurrentTime;
            }
        }

    public abstract class BaseAuditableEntitySingleUser : AuditableBaseEntity
        {
        public override Guid UserId { get; set; } //only this will be used

        //below will be excluded
        [NotMapped]
        //"Don't use this as not required & hiding with private", true)]
        public override Guid CreatedBy { get; set; }


        [NotMapped]
        //[Obsolete("Don't use this as not required & hiding with private", true)]
        public override Guid? LastModifiedBy { get; set; }

        }
    public abstract class BaseAuditableEntityMultiUser : AuditableBaseEntity
        {
        public override Guid CreatedBy { get; set; }
        public override Guid? LastModifiedBy { get; set; }

        //below will be excluded
        //[Obsolete("Don't use this as not required & hiding with private", true)]
        [NotMapped]
        public override Guid UserId { get; set; }
        }


    }
