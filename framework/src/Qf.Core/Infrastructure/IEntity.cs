using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Infrastructure
{
    public interface IEntity
    {
    }
    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; }
    }
}
