using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Infrastructure
{
    public abstract class FullAuditedEntity : AuditedEntity
    {
        public virtual bool IsDeleted { get; set; }

        public virtual Guid? DeleterId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }
    }
    public abstract class FullAuditedEntity<TKey> : AuditedEntity<TKey>
    {
        public virtual bool IsDeleted { get; set; }

        public virtual Guid? DeleterId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }

        protected FullAuditedEntity()
        {

        }

        protected FullAuditedEntity(TKey id)
            : base(id)
        {

        }
    }
}
