using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Infrastructure
{
    public abstract class Entity<T>
    {
        public T Id { get; set; }
    }
}
