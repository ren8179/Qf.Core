using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Infrastructure
{
    [Serializable]
    public abstract class Entity
    {
    }
    public abstract class Entity<TKey> : Entity
    {
        public virtual TKey Id { get; set; }
        protected Entity()
        {

        }
        protected Entity(TKey id)
        {
            Id = id;
        }
    }
}
