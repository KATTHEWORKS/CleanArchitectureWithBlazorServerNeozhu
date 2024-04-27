using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicCommon
    {
    public abstract class BaseAuditableEntitySingleUser : BaseAuditableEntity
        {

        }
    public abstract class BaseAuditableEntityMultiUser : BaseAuditableEntity
        {
        public virtual string? CreatedBy { get; set; }
        public virtual string? UpdatedBy { get; set; }
        }
    public abstract class BaseAuditableEntity
        {
        public virtual int Id { get; set; }
        public virtual DateTime? Created { get; set; }

        public virtual DateTime? Updated { get; set; }

        }

    }
