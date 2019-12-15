using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Infrastructure
{
    [Serializable]
    public abstract class FullAuditedEntity : AuditedEntity
    {
        public virtual bool IsDeleted { get; set; }

        public virtual Guid? DeleterId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }
    }
    [Serializable]
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
