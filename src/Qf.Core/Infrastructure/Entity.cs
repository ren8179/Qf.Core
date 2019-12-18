using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Infrastructure
{
    public abstract class Entity : IEntity
    {
    }
    public abstract class Entity<TKey> : Entity, IEntity<TKey>
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
